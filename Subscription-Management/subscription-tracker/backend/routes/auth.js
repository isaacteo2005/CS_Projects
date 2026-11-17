/*
 * Defines API endpoints for user authentication:
 * Routes define the API endpoints
 * POST /api/auth/register - Create new user account
 * POST /api/auth/login    - Authenticate existing user
 * 
 * Each route calls the corresponding controller function
*/
import express from 'express';
import { login, register } from '../controllers/userController.js';

const router = express.Router();

router.post('/login', login);
router.post('/register', register);

export default router;