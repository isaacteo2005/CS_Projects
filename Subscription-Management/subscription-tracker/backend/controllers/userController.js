import { supabase } from '../config/supabase.js';
import bcrypt from 'bcrypt';

/**
 * USER LOGIN
 * Verifies credentials and returns user data
 */
export const login = async (req, res) => {
    const { email, password } = req.body;
    
    if (!email || !password) {
        return res.status(400).json({ error: 'Email and password are required' });
    }
    
    try {
        // Find user by email
        const { data: users, error: findError } = await supabase
            .from('users')
            .select('*')
            .eq('email', email);
        
        if (findError) throw findError;
        
        if (!users || users.length === 0) {
            return res.status(401).json({ error: 'Invalid credentials' });
        }
        
        const user = users[0];
        
        // Compare password
        const isPasswordValid = await bcrypt.compare(password, user.password);
        
        if (!isPasswordValid) {
            return res.status(401).json({ error: 'Invalid credentials' });
        }
        
        // First get current stats
        const { data: currentStats, error: statsError } = await supabase
            .from('user_stats')
            .select('session_count, overall_spent')
            .eq('user_id', user.user_id)
            .single();
        
        if (statsError && statsError.code !== 'PGRST116') {
            console.error('Error fetching stats:', statsError);
        }
        
        const newSessionCount = (currentStats?.session_count || 0) + 1;
        const today = new Date().toISOString().split('T')[0];
        
        // Update user_stats
        const { error: updateError } = await supabase
            .from('user_stats')
            .upsert({
                user_id: user.user_id,
                last_login: today,
                session_count: newSessionCount,
                overall_spent: currentStats?.overall_spent || 0
            }, { onConflict: 'user_id' });
        
        if (updateError) {
            console.error('Error updating stats:', updateError);
            // Don't fail login if stats update fails
        }
        
        // Return user data without password
        const { password: _, ...userWithoutPassword } = user;
        res.json({ 
            success: true, 
            user: userWithoutPassword
        });
        
    } catch (error) {
        console.error('Error during login:', error);
        res.status(500).json({ error: 'Server error during login' });
    }
};

/**
 * USER REGISTRATION
 * Creates a new user account
 */
export const register = async (req, res) => {
    const { name, email, password } = req.body;
    
    // Basic validation
    if (!name || !email || !password) {
        return res.status(400).json({ error: 'Name, email, and password are required' });
    }
    
    try {
        // Check if email already exists
        const { data: existingUsers, error: checkError } = await supabase
            .from('users')
            .select('user_id')
            .eq('email', email);
        
        if (checkError) throw checkError;
        
        if (existingUsers && existingUsers.length > 0) {
            return res.status(409).json({ error: 'Email already registered' });
        }
        
        // Hash the password
        const hashedPassword = await bcrypt.hash(password, 10);
        
        // Insert new user
        const { data: newUser, error: insertError } = await supabase
            .from('users')
            .insert([{
                name: name,
                email: email,
                password: hashedPassword,
                two_factor_bool: false
            }])
            .select()
            .single();
        
        if (insertError) throw insertError;
        
        // user_stats record is created automatically by the trigger
        // No need to manually insert
        
        res.json({ 
            success: true, 
            userId: newUser.user_id,
            message: 'Registration successful'
        });
        
    } catch (error) {
        console.error('Error during register:', error);
        
        // Check for duplicate email error
        if (error.code === '23505') { // PostgreSQL duplicate key error code
            return res.status(409).json({ error: 'Email already registered' });
        }
        
        res.status(500).json({ error: 'Server error during registration' });
    }
};

/**
 * GET USER BY ID
 * Fetches user details for a given user ID
 */
export const getUserById = async (req, res) => {
    const { userId } = req.params;
    
    try {
        const { data: users, error } = await supabase
            .from('users')
            .select('user_id, name, email, created_at, two_factor_bool')
            .eq('user_id', userId);
        
        if (error) throw error;
        
        if (!users || users.length === 0) {
            return res.status(404).json({ error: 'User not found' });
        }
        
        res.json(users[0]);
    } catch (error) {
        console.error('Error fetching user:', error);
        res.status(500).json({ error: 'Server error fetching user' });
    }
};

/**
 * DELETE USER ACCOUNT
 * Permanently deletes a user and all associated data
 */
export const deleteUserAccount = async (req, res) => {
    const { userId } = req.params;
    
    console.log('🗑️ Deleting account for user ID:', userId);
    
    try {
        // Check if user exists
        const { data: users, error: findError } = await supabase
            .from('users')
            .select('user_id, email')
            .eq('user_id', userId);
        
        if (findError) throw findError;
        
        if (!users || users.length === 0) {
            return res.status(404).json({ error: 'User not found' });
        }
        
        console.log('📧 Deleting user:', users[0].email);
        
        // Delete user - CASCADE will handle subscriptions, stats, and logs
        const { error: deleteError } = await supabase
            .from('users')
            .delete()
            .eq('user_id', userId);
        
        if (deleteError) throw deleteError;
        
        console.log('✅ Account deleted successfully');
        
        res.json({ 
            success: true, 
            message: 'Account deleted successfully' 
        });
        
    } catch (error) {
        console.error('Error deleting account:', error);
        res.status(500).json({ error: 'Server error deleting account' });
    }
};
