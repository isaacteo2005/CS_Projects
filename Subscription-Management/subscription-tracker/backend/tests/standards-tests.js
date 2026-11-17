import dotenv from 'dotenv';
import path from 'path';
import { fileURLToPath } from 'url';
import mysql from 'mysql2/promise';
import bcrypt from 'bcrypt';

// Load environment variables from .env file
const __dirname = path.dirname(fileURLToPath(import.meta.url));
dotenv.config({ path: path.join(__dirname, '../.env') });

console.log('═══════════════════════════════════════════');
console.log('   DATABASE CONNECTION CHECK');
console.log('═══════════════════════════════════════════');
console.log('DB_USER:', process.env.DB_USER);
console.log('DB_PASSWORD:', process.env.DB_PASSWORD ? '✅ Loaded' : '❌ Not loaded');
console.log('DB_NAME:', process.env.DB_NAME);
console.log('DB_HOST:', process.env.DB_HOST);

// Create connection pool for testing
const testPool = mysql.createPool({
    host: process.env.DB_HOST || 'localhost',
    user: process.env.DB_USER || 'root',
    password: process.env.DB_PASSWORD,
    database: process.env.DB_NAME || 'subscription_tracker',
    waitForConnections: true,
    connectionLimit: 5,
    queueLimit: 0
});

async function runSimpleTests() {
    console.log('\n═══════════════════════════════════════════');
    console.log('   SIMPLE SECURITY & DATA PRIVACY TESTS');
    console.log('═══════════════════════════════════════════\n');

    // Test database connection first
    try {
        const [result] = await testPool.query('SELECT 1 + 1 AS solution');
        console.log('✅ Database connected successfully!');
        console.log(`   Test query result: ${result[0].solution}`);
        console.log(`   Using database: ${process.env.DB_NAME}`);
        console.log(`   As user: ${process.env.DB_USER}\n`);
    } catch (error) {
        console.log('❌ Database connection failed:', error.message);
        console.log('\n🔧 Troubleshooting:');
        console.log('1. Check that MySQL is running');
        console.log('2. Verify your .env file has the correct password');
        console.log('3. Try connecting manually: mysql -u root -p');
        process.exit(1);
    }

    const testEmail = `test${Date.now()}@test.com`;
    const testPassword = 'MySecretPassword123!';
    let userId;

    // TEST 1: Password Hashing: tests that passwords are hashed and can be verified correctly
    console.log('📝 TEST 1: Password Hashing');
    console.log('─────────────────────────');
    
    try {
        const hashedPassword = await bcrypt.hash(testPassword, 10);
        console.log('   Original password:', testPassword);
        console.log('   Hashed password:', hashedPassword);
        console.log('   Hash length:', hashedPassword.length);
        console.log('   ✅ Password can be hashed successfully');
        
        const isValid = await bcrypt.compare(testPassword, hashedPassword);
        console.log('   ✅ Password comparison works:', isValid);
        
        const isWrongValid = await bcrypt.compare('wrongpassword', hashedPassword);
        console.log('   ✅ Wrong password rejected:', !isWrongValid);
        
    } catch (error) {
        console.log('   ❌ Password hashing failed:', error.message);
    }
    console.log('');

    // TEST 2: Account Creation & Deletion: tests that a user can be created, verified, and deleted properly
    console.log('📝 TEST 2: Account Creation & Deletion');
    console.log('─────────────────────────');
    
    try {
        const hashedPwd = await bcrypt.hash(testPassword, 10);
        const [insertResult] = await testPool.query(
            'INSERT INTO users (name, email, password, created_at, two_factor_bool) VALUES (?, ?, ?, CURDATE(), FALSE)',
            ['Test User', testEmail, hashedPwd]
        );
        
        userId = insertResult.insertId;
        console.log('   ✅ Test user created with ID:', userId);
        
        const [users] = await testPool.query('SELECT * FROM users WHERE user_id = ?', [userId]);
        console.log('   ✅ User found in database:', users.length > 0);
        
        const storedUser = users[0];
        const loginValid = await bcrypt.compare(testPassword, storedUser.password);
        console.log('   ✅ Login verification works:', loginValid);
        
        await testPool.query('DELETE FROM user_stats WHERE user_id = ?', [userId]);
        await testPool.query('DELETE FROM users WHERE user_id = ?', [userId]);
        
        const [checkDeleted] = await testPool.query('SELECT * FROM users WHERE user_id = ?', [userId]);
        console.log('   ✅ User successfully deleted:', checkDeleted.length === 0);
        
    } catch (error) {
        console.log('   ❌ Account test failed:', error.message);
    }
    console.log('');

    // TEST 3: Data Privacy Check with Auto-Fail
console.log('📝 TEST 3: Data Privacy Check');
console.log('─────────────────────────');

try {
    const tempPwd = await bcrypt.hash('temp123', 10);
    const [tempResult] = await testPool.query(
        'INSERT INTO users (name, email, password, created_at, two_factor_bool) VALUES (?, ?, ?, CURDATE(), FALSE)',
        ['Privacy Test', `privacy${Date.now()}@test.com`, tempPwd]
    );
    
    const tempId = tempResult.insertId;
    
    const [userData] = await testPool.query('SELECT * FROM users WHERE user_id = ?', [tempId]);
    const user = userData[0];
    
    // Define what fields are ALLOWED
    const allowedFields = [
        'user_id', 
        'name', 
        'email', 
        'password', 
        'created_at', 
        'two_factor_bool'
    ];
    
    // Get actual fields from database
    const actualFields = Object.keys(user);
    
    console.log('   Allowed fields:', allowedFields.join(', '));
    console.log('   Actual fields:  ', actualFields.join(', '));
    
    // Find any fields that are NOT in the allowed list
    const unexpectedFields = actualFields.filter(
        field => !allowedFields.includes(field)
    );
    
    if (unexpectedFields.length > 0) {
        console.log(`   ❌ PRIVACY VIOLATION! Unexpected fields: ${unexpectedFields.join(', ')}`);
        console.log('   These fields should not be stored!');
        
        // Optional: Clean up before failing
        await testPool.query('DELETE FROM users WHERE user_id = ?', [tempId]);
        
        // Mark test as failed (you'd need to track this)
        testsPassed = false;
        privacyTestFailed = true;
    } else {
        console.log('   ✅ Only necessary data stored (no unexpected fields)');
        await testPool.query('DELETE FROM users WHERE user_id = ?', [tempId]);
    }
    
} catch (error) {
    console.log('   ❌ Privacy check failed:', error.message);
    testsPassed = false;
}
    console.log('');

    // TEST 4: User Stats Creation: tests that user stats are created with correct fields and default values
    console.log('📝 TEST 4: User Stats Creation');
    console.log('─────────────────────────');
    
    try {
        const statsPwd = await bcrypt.hash('stats123', 10);
        const [statsResult] = await testPool.query(
            'INSERT INTO users (name, email, password, created_at, two_factor_bool) VALUES (?, ?, ?, CURDATE(), FALSE)',
            ['Stats Test', `stats${Date.now()}@test.com`, statsPwd]
        );
        
        const statsUserId = statsResult.insertId;
        
        await testPool.query(
            'INSERT INTO user_stats (user_id, overall_spent, last_login, session_count) VALUES (?, 0, CURDATE(), 0)',
            [statsUserId]
        );
        
        const [stats] = await testPool.query('SELECT * FROM user_stats WHERE user_id = ?', [statsUserId]);
        console.log('   ✅ User stats created:', stats.length > 0);
        console.log('   Stats fields:', Object.keys(stats[0] || {}).join(', '));
        
        await testPool.query('DELETE FROM user_stats WHERE user_id = ?', [statsUserId]);
        await testPool.query('DELETE FROM users WHERE user_id = ?', [statsUserId]);
        
    } catch (error) {
        console.log('   ❌ Stats creation test failed:', error.message);
    }
    console.log('');

    // TEST 5: Direct Password Verification
    console.log('📝 TEST 5: Direct Password Check');
    console.log('─────────────────────────');
    
    try {
        // Create a user
        const pwd = 'test12345';
        const hashed = await bcrypt.hash(pwd, 10);
        
        const [result] = await testPool.query(
            'INSERT INTO users (name, email, password, created_at, two_factor_bool) VALUES (?, ?, ?, CURDATE(), FALSE)',
            ['Password Test', `pwd${Date.now()}@test.com`, hashed]
        );
        
        const pwdUserId = result.insertId;
        
        // Retrieve and verify
        const [users] = await testPool.query('SELECT password FROM users WHERE user_id = ?', [pwdUserId]);
        const verify = await bcrypt.compare(pwd, users[0].password);
        
        console.log('   ✅ Password hash/verify cycle works:', verify);
        
        // Clean up
        await testPool.query('DELETE FROM users WHERE user_id = ?', [pwdUserId]);
        
    } catch (error) {
        console.log('   ❌ Password verification test failed:', error.message);
    }
    console.log('');

   

    await testPool.end();
    process.exit(0);
}

// Run the tests
runSimpleTests().catch(error => {
    console.error('❌ Test failed with error:', error);
    process.exit(1);
});