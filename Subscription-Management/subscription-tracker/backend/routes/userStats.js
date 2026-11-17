import express from 'express';
import { getUserStats, updateUserStats } from '../controllers/userStatsController.js';

const router = express.Router();

// Get user stats by user ID
router.get('/:userId', getUserStats);

// Update user stats
router.put('/:userId', updateUserStats);

export default router;