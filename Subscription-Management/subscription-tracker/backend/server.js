/*
This is the main server file that:
 * - Initializes the Express application
 * - Sets up middleware (CORS, JSON parsing)
 * - Connects all routes
 * - Starts the server on port 3001
 * central place for API requests
*/
import express from 'express';
import cors from 'cors';
import authRoutes from './routes/auth.js';
import userRoutes from './routes/users.js';
import subscriptionRoutes from './routes/subscriptions.js';
import userStatsRoutes from './routes/userStats.js';
// This file creates the Express server, adds middleware, and connects all the API routes.
const app = express();

app.use(cors());
app.use(express.json());

// Test route
app.get('/test', (req, res) => {
    res.json({ message: 'Server is working!' });
});
// Use routes
app.use('/api/auth', authRoutes);
app.use('/api/users', userRoutes); 
app.use('/api/subscriptions', subscriptionRoutes);
app.use('/api/user-stats', userStatsRoutes);

// Only start the server if not in test environment
if (process.env.NODE_ENV !== 'test') {
    const PORT = process.env.PORT || 3001;
    app.listen(PORT, () => {
        console.log(`Server running on port ${PORT}`);
    });
}

export default app;
