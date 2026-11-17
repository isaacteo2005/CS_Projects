import dotenv from 'dotenv';
import path from 'path';
import { fileURLToPath } from 'url';
import mysql from 'mysql2/promise';
import bcrypt from 'bcrypt';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
dotenv.config({ path: path.join(__dirname, '../.env') });

// Create connection pool
const pool = mysql.createPool({
    host: process.env.DB_HOST || 'localhost',
    user: process.env.DB_USER || 'root',
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME || 'subscription_tracker',
    waitForConnections: true,
    connectionLimit: 5,
    queueLimit: 0
});

// Helper to measure time
async function measureTime(name, fn) {
    const start = Date.now();
    await fn();
    const duration = Date.now() - start;
    console.log(`   ${name}: ${duration}ms ${duration < 200 ? '✅' : duration < 500 ? '⚠️' : '❌'}`);
    return duration;
}

async function runPerformanceTests() {
    console.log('═══════════════════════════════════════════');
    console.log('      PERFORMANCE TESTS');
    console.log('═══════════════════════════════════════════\n');

    const results = {};

    // TEST 1: Database Connection Speed
    console.log('📊 TEST 1: Database Connection');
    console.log('─────────────────────────');
    results.dbConnect = await measureTime('Connect & Query', async () => {
        const [result] = await pool.query('SELECT 1 + 1 AS solution');
        if (result[0].solution !== 2) throw new Error('Wrong result');
    });
    console.log('');

    // TEST 2: Password Hashing Speed
    console.log('📊 TEST 2: Password Hashing');
    console.log('─────────────────────────');
    results.passwordHash = await measureTime('Hash password (10 rounds)', async () => {
        await bcrypt.hash('testpassword123', 10);
    });
    
    results.passwordCompare = await measureTime('Compare password', async () => {
        const hash = await bcrypt.hash('testpassword123', 10);
        await bcrypt.compare('testpassword123', hash);
    });
    console.log('');

    // TEST 3: User Registration Speed
    console.log('📊 TEST 3: User Registration');
    console.log('─────────────────────────');
    const testEmail = `perf${Date.now()}@test.com`;
    let userId;
    
    results.registration = await measureTime('Complete registration', async () => {
        const hashedPwd = await bcrypt.hash('password123', 10);
        const [result] = await pool.query(
            'INSERT INTO users (name, email, password, created_at, two_factor_bool) VALUES (?, ?, ?, CURDATE(), FALSE)',
            ['Performance Test', testEmail, hashedPwd]
        );
        userId = result.insertId;
        
        await pool.query(
            'INSERT INTO user_stats (user_id, overall_spent, last_login, session_count) VALUES (?, 0, CURDATE(), 0)',
            [userId]
        );
    });
    console.log('');

    // TEST 4: User Login Speed
    console.log('📊 TEST 4: User Login Simulation');
    console.log('─────────────────────────');
    results.userLookup = await measureTime('Find user by email', async () => {
        const [users] = await pool.query('SELECT * FROM users WHERE user_id = ?', [userId]);
        if (users.length === 0) throw new Error('User not found');
    });
    console.log('');

    // TEST 5: Analytics Data Retrieval
    console.log('📊 TEST 5: Analytics Data');
    console.log('─────────────────────────');
    results.analytics = await measureTime('Get user stats', async () => {
        const [stats] = await pool.query('SELECT * FROM user_stats WHERE user_id = ?', [userId]);
        if (stats.length === 0) throw new Error('Stats not found');
    });
    console.log('');

    // TEST 6: Multiple Concurrent Queries
    console.log('📊 TEST 6: Concurrent Queries');
    console.log('─────────────────────────');
    const start = Date.now();
    const promises = [];
    for (let i = 0; i < 5; i++) {
        promises.push(pool.query('SELECT * FROM users WHERE user_id = ?', [userId]));
        promises.push(pool.query('SELECT * FROM user_stats WHERE user_id = ?', [userId]));
    }
    await Promise.all(promises);
    const concurrentTime = Date.now() - start;
    console.log(`   5 concurrent reads: ${concurrentTime}ms ${concurrentTime < 500 ? '✅' : '❌'}`);
    results.concurrent = concurrentTime;
    console.log('');

    // TEST 7: Clean up - Delete test user
    console.log('📊 TEST 7: Cleanup');
    console.log('─────────────────────────');
    await measureTime('Delete test data', async () => {
        await pool.query('DELETE FROM user_stats WHERE user_id = ?', [userId]);
        await pool.query('DELETE FROM users WHERE user_id = ?', [userId]);
    });
    console.log('');

    // Summary
    console.log('═══════════════════════════════════════════');
    console.log('📊 PERFORMANCE SUMMARY');
    console.log('═══════════════════════════════════════════');
    
    const totalTime = Object.values(results).reduce((a, b) => a + b, 0);
    console.log(`Total test time: ${totalTime}ms\n`);

    console.log('✅ Excellent (<200ms):');
    Object.entries(results).forEach(([key, time]) => {
        if (time < 200) console.log(`   ${key}: ${time}ms`);
    });

    console.log('\n⚠️  Acceptable (200-500ms):');
    Object.entries(results).forEach(([key, time]) => {
        if (time >= 200 && time < 500) console.log(`   ${key}: ${time}ms`);
    });

    console.log('\n❌ Slow (>500ms):');
    Object.entries(results).forEach(([key, time]) => {
        if (time >= 500) console.log(`   ${key}: ${time}ms`);
    });

    console.log('\n═══════════════════════════════════════════');
    console.log('RECOMMENDATIONS:');
    if (results.passwordHash > 300) {
        console.log('• Password hashing is slow - consider reducing bcrypt rounds');
    }
    if (results.analytics > 200) {
        console.log('• Analytics query is slow - consider adding indexes');
    }
    if (results.concurrent > 1000) {
        console.log('• Database struggles with concurrency - check connection pool');
    }
    if (Object.values(results).some(t => t > 500)) {
        console.log('• Some operations are slow - optimize queries');
    } else {
        console.log('✅ All performance tests passed!');
    }
    console.log('═══════════════════════════════════════════');

    await pool.end();
}

runPerformanceTests().catch(console.error);