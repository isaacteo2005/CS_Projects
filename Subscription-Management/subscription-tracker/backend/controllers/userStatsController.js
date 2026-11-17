import { supabase } from '../config/supabase.js';

/**
 * GET USER STATS
 * Fetches statistics for a specific user
 */
export const getUserStats = async (req, res) => {
    const { userId } = req.params;
    
    try {
        const { data: stats, error } = await supabase
            .from('user_stats')
            .select('*')
            .eq('user_id', userId)
            .single();
        
        if (error && error.code !== 'PGRST116') { // PGRST116 = no rows returned
            throw error;
        }
        
        if (!stats) {
            // Create default stats if none exist
            const today = new Date().toISOString().split('T')[0];
            const { data: newStats, error: insertError } = await supabase
                .from('user_stats')
                .insert([{
                    user_id: parseInt(userId),
                    overall_spent: 0,
                    last_login: today,
                    session_count: 0
                }])
                .select()
                .single();
            
            if (insertError) throw insertError;
            
            return res.json(newStats);
        }
        
        res.json(stats);
        
    } catch (error) {
        console.error('Error fetching user stats:', error);
        res.status(500).json({ error: 'Server error fetching stats' });
    }
};

/**
 * UPDATE USER STATS
 * Updates statistics for a user
 */
export const updateUserStats = async (req, res) => {
    const { userId } = req.params;
    const updates = req.body;
    
    try {
        const { data: updated, error } = await supabase
            .from('user_stats')
            .update(updates)
            .eq('user_id', userId)
            .select()
            .single();
        
        if (error) throw error;
        
        res.json({ 
            success: true, 
            stats: updated 
        });
        
    } catch (error) {
        console.error('Error updating user stats:', error);
        res.status(500).json({ error: 'Server error updating stats' });
    }
};

/**
 * GET LEADERBOARD
 * Returns top users by spending
 */
export const getLeaderboard = async (req, res) => {
    try {
        const { data: leaderboard, error } = await supabase
            .from('user_stats')
            .select(`
                user_id,
                overall_spent,
                session_count,
                users!inner (
                    name,
                    email
                )
            `)
            .order('overall_spent', { ascending: false })
            .limit(10);
        
        if (error) throw error;
        
        res.json(leaderboard);
        
    } catch (error) {
        console.error('Error fetching leaderboard:', error);
        res.status(500).json({ error: 'Server error fetching leaderboard' });
    }
};