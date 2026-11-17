import express from 'express';
import { getUserById, deleteUserAccount } from '../controllers/userController.js';

const router = express.Router();

router.get('/:userId', getUserById);
router.delete('/:userId', deleteUserAccount);

export default router;