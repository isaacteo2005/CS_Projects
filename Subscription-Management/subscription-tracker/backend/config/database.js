/*
- Creates a connection pool to MySQL database
 * - Loads environment variables for credentials
 * - Exports the pool for use in controllers
 * 
 * Connected to database: subscription_tracker
*/
import mysql from 'mysql2/promise';
import dotenv from 'dotenv';

dotenv.config();

const pool = mysql.createPool({
    host: process.env.DB_HOST || 'localhost',
    user: process.env.DB_USER || 'root',
    password: process.env.DB_PASSWORD || '',
    database: process.env.DB_NAME || 'subscription_tracker',
    waitForConnections: true,
    connectionLimit: 10,
    queueLimit: 0
});

export default pool;