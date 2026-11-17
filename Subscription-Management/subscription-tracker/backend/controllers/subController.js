/*
* Handles business logic for subscription operations: for the endpoints 
 * 
 * getSubscriptions()    - Fetches all subscriptions for a user
 * addSubscription()     - Creates new subscription record
 * updateSubscription()  - Updates existing subscription
 * deleteSubscription()  - Deletes subscription
*/

import { supabase } from '../config/supabase.js';

const parseCurrency = (value) => {
    // Accept numbers or strings, normalize commas to dots, trim whitespace
    let normalizedValue = String(value ?? '').replace(',', '.').trim();
    // Accept numbers like .90, 0.90, 7.90, 7, 7.0, 7.00
    if (/^\d*(\.\d{1,2})?$/.test(normalizedValue) && normalizedValue !== '') {
        const parsedValue = Number.parseFloat(normalizedValue);
        return Number.isFinite(parsedValue) ? parsedValue : null;
    }
    return null;
};

const isValidDate = (value) => /^\d{4}-\d{2}-\d{2}$/.test(String(value ?? '').trim());

const syncUserStats = async (userId, extraUpdates = {}) => {
    const { data: subscriptions, error: subscriptionsError } = await supabase
        .from('subscriptions')
        .select('subscription_id, company_name, renewal_cost')
        .eq('user_id', userId);

    if (subscriptionsError) {
        throw subscriptionsError;
    }

    const overallSpent = (subscriptions || []).reduce(
        (total, subscription) => total + Number(subscription.renewal_cost || 0),
        0
    );

    const mostExpensiveSubscription = (subscriptions || []).reduce((currentMax, subscription) => {
        if (!currentMax) {
            return subscription;
        }

        return Number(subscription.renewal_cost || 0) > Number(currentMax.renewal_cost || 0)
            ? subscription
            : currentMax;
    }, null);

    const statsUpdates = {
        overall_spent: overallSpent,
        most_spenton: mostExpensiveSubscription?.subscription_id ?? null,
        most_spentname: mostExpensiveSubscription?.company_name ?? null,
        ...extraUpdates
    };

    const { error: statsError } = await supabase
        .from('user_stats')
        .upsert({
            user_id: Number(userId),
            ...statsUpdates
        }, { onConflict: 'user_id' });

    if (statsError) {
        throw statsError;
    }
};

/**
 * GET SUBSCRIPTIONS BY USER ID
 * Fetches all subscriptions for a specific user
 */
export const getSubscriptions = async (req, res) => {
    const { userId } = req.params;
    
    try {
        const { data: subscriptions, error } = await supabase
            .from('subscriptions')
            .select('*')
            .eq('user_id', userId)
            .order('subscription_id', { ascending: false });
        
        if (error) throw error;
        
        res.json(subscriptions || []);
    } catch (error) {
        console.error('Error fetching subscriptions:', error);
        res.status(500).json({ error: 'Server error fetching subscriptions' });
    }
};

/**
 * ADD SUBSCRIPTION
 * Creates a new subscription for a user
 */
export const addSubscription = async (req, res) => {
    const { user_id, company_name, renewal_cost, sub_end, total_spent = 0 } = req.body;
    const parsedRenewalCost = parseCurrency(renewal_cost);
    const parsedTotalSpent = parseCurrency(total_spent);

    if (!user_id || !company_name?.trim()) {
        return res.status(400).json({ error: 'User and company name are required.' });
    }

    if (parsedRenewalCost === null) {
        return res.status(400).json({ error: 'Renewal cost must be a valid amount with up to 2 decimal places.' });
    }

    if (!isValidDate(sub_end)) {
        return res.status(400).json({ error: 'Renewal date must be a valid date in YYYY-MM-DD format.' });
    }

    if (parsedTotalSpent === null) {
        return res.status(400).json({ error: 'Total spent must be a valid amount with up to 2 decimal places.' });
    }
    
    try {
        const { data: newSubscription, error } = await supabase
            .from('subscriptions')
            .insert([{
                user_id: user_id,
                company_name: company_name.trim(),
                renewal_cost: parsedRenewalCost,
                sub_end: sub_end,
                total_spent: parsedTotalSpent,
                sub_start: new Date().toISOString().split('T')[0],
                ongoing: true
            }])
            .select()
            .single();
        
        if (error) throw error;

        await syncUserStats(user_id);
        
        res.json({ 
            success: true, 
            subscription: newSubscription 
        });
        
    } catch (error) {
        console.error('Error adding subscription:', error);
        res.status(500).json({ error: error?.message || 'Server error adding subscription' });
    }
};

/**
 * DELETE SUBSCRIPTION
 * Removes a subscription by ID
 */
export const deleteSubscription = async (req, res) => {
    const { subscriptionId } = req.params;
    
    try {
        // First get the subscription to know user_id for stats update
        const { data: sub, error: fetchError } = await supabase
            .from('subscriptions')
            .select('user_id, company_name')
            .eq('subscription_id', subscriptionId)
            .single();
        
        if (fetchError) throw fetchError;
        
        // Delete the subscription
        const { error: deleteError } = await supabase
            .from('subscriptions')
            .delete()
            .eq('subscription_id', subscriptionId);
        
        if (deleteError) throw deleteError;
        
        await syncUserStats(sub.user_id, {
            last_deleted: null,
            last_deletedname: sub.company_name
        });
        
        res.json({ 
            success: true, 
            message: 'Subscription deleted successfully' 
        });
        
    } catch (error) {
        console.error('Error deleting subscription:', error);
        res.status(500).json({ error: 'Server error deleting subscription' });
    }
};

/**
 * UPDATE SUBSCRIPTION
 * Updates an existing subscription
 */
export const updateSubscription = async (req, res) => {
    const { subscriptionId } = req.params;
    const updates = { ...req.body };

    if (Object.hasOwn(updates, 'renewal_cost')) {
        const parsedRenewalCost = parseCurrency(updates.renewal_cost);

        if (parsedRenewalCost === null) {
            return res.status(400).json({ error: 'Renewal cost must be a valid amount with up to 2 decimal places.' });
        }

        updates.renewal_cost = parsedRenewalCost;
    }

    if (Object.hasOwn(updates, 'total_spent')) {
        const parsedTotalSpent = parseCurrency(updates.total_spent);

        if (parsedTotalSpent === null) {
            return res.status(400).json({ error: 'Total spent must be a valid amount with up to 2 decimal places.' });
        }

        updates.total_spent = parsedTotalSpent;
    }

    if (Object.hasOwn(updates, 'sub_end')) {
        if (!isValidDate(updates.sub_end)) {
            return res.status(400).json({ error: 'Renewal date must be a valid date in YYYY-MM-DD format.' });
        }
    }

    if (Object.hasOwn(updates, 'company_name') && !String(updates.company_name ?? '').trim()) {
        return res.status(400).json({ error: 'Company name cannot be empty.' });
    }
    
    try {
        const { data: existingSubscription, error: fetchError } = await supabase
            .from('subscriptions')
            .select('user_id')
            .eq('subscription_id', subscriptionId)
            .single();

        if (fetchError) throw fetchError;

        const { data: updated, error } = await supabase
            .from('subscriptions')
            .update(updates)
            .eq('subscription_id', subscriptionId)
            .select()
            .single();
        
        if (error) throw error;

        await syncUserStats(existingSubscription.user_id);
        
        res.json({ 
            success: true, 
            subscription: updated 
        });
        
    } catch (error) {
        console.error('Error updating subscription:', error);
        res.status(500).json({ error: 'Server error updating subscription' });
    }
};
