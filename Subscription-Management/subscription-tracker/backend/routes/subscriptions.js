/*
 * Defines API endpoints for managing subscriptions:
 * 
 * GET    /api/subscriptions/user/:userId    - Get user's subscriptions
 * POST   /api/subscriptions                  - Add new subscription
 * PUT    /api/subscriptions/:subscriptionId  - Update subscription
 * DELETE /api/subscriptions/:subscriptionId  - Delete subscription
*/
import express from 'express';
import {
    getSubscriptions,
    addSubscription,
    updateSubscription,
    deleteSubscription
} from '../controllers/subController.js';

const router = express.Router();

// Get all subscriptions for a user
router.get('/user/:userId', getSubscriptions);

// Add a new subscription
router.post('/', addSubscription);

// Update a subscription
router.put('/:subscriptionId', updateSubscription);

// Delete a subscription
router.delete('/:subscriptionId', deleteSubscription);

export default router;
