
//unit testing 
import bcrypt from 'bcrypt';
import pool from '../config/database.js';
import { login, register } from '../controllers/userController.js';
import { addSubscription } from '../controllers/subController.js';

function makeRes() {
    return {
        statusCode: 200,
        body: null,
        status(code) {
            this.statusCode = code;
            return this;
        },
        json(data) {
            this.body = data;
            return this;
        }
    };
}

function printTest(name, expected, observed, passed) {
    console.log(`\n${name}`);
    console.log(`Expected: ${expected}`);
    console.log(`Observed: ${observed}`);
    console.log(`Result: ${passed ? 'Pass' : 'Fail'}`);
}

async function removeUserData(userId) 

{
    if (!userId) return;
    await pool.query('DELETE FROM subscriptions WHERE user_id = ?', [userId]);
    await pool.query('DELETE FROM user_stats WHERE user_id = ?', [userId]);
    await pool.query('DELETE FROM users WHERE user_id = ?', [userId]);
}

async function createUser(tag) {
    const email = `${tag}.${Date.now()}@test.com`;
    const password = 'TestPassword123!';
    const req = {
        body: {
            name: 'Test User',
            email,
            password
        }
    };
    const res = makeRes();

    await register(req, res);

  return {
      userId: res.body?.userId,
       email,
       password,
      ok: Boolean(res.body?.success)
    };
}

async function passwordHashTest() {
    let userId;

    try {
     const user = await createUser('hash');
     userId = user.userId;

    if (!user.ok) {
            return printTest(
          '1. Password Hash Function',
         'Password should be hashed before saving',
           'User registration did not complete successfully',
                false
            );
        }

  const [rows] = await pool.query('SELECT password FROM users WHERE user_id = ?', [userId]);
   const savedPassword = rows[0]?.password;
   const matches = await bcrypt.compare(user.password, savedPassword);
   const passed = savedPassword !== user.password && matches;

     printTest(
          '1. Password Hash Function',
          'Password should be hashed before saving',
          passed ? 'Password has been successfully hashed' : 'Password was not hashed correctly',
           passed
        );
    } catch (error) {
     printTest(
       '1. Password Hash Function',
     'Password should be hashed before saving',
       error.message,
      false
        );
    } finally {
        await removeUserData(userId);
    }
}

async function totalSpendingTest() {
    let userId;

    try {
        const user = await createUser('total');
        userId = user.userId;

    if (!user.ok) {        return printTest(
           '2. Total Spending Calculation',
          'Function should return the correct total cost of subscriptions',
         'User registration did not complete successfully',
          false
            );
        }

        const req = {
            body: {
        user_id: userId,
          company_name: `TotalCheck-${Date.now()}`,
          renewal_cost: 19.99,
             sub_end: '2026-12-31'
            }
        };
        const res = makeRes();

        await addSubscription(req, res);

        const [rows] = await pool.query(
            'SELECT overall_spent FROM user_stats WHERE user_id = ?',
            [userId]
        );
    const total = Number(rows[0]?.overall_spent ?? 0);
   const passed = total === 19.99;

  printTest(
      '2. Total Spending Calculation',
      'Function should return the correct total cost of subscriptions',
       passed ? `Total spending saved correctly: ${total}` : `Total spending does not correctly work: ${total}`,
       passed
        );
    } catch (error) {
        printTest(
     '2. Total Spending Calculation',
      'Function should return the correct total cost of subscriptions',
       error.message,
      false
        );
    } finally {
        await removeUserData(userId);
    }
}

async function subscriptionInsertTest() {
    let userId;

    try {
        const user = await createUser('insert');
  userId = user.userId;

   if (!user.ok) {
           return printTest(
          '3. Subscription Insert Validation',
           'Valid data should pass validation and proceed to database insertion',
         'User registration did not complete successfully',
                false      );
   }

   const companyName = `InsertCheck-${Date.now()}`;
    const req = {
     body: {
         user_id: userId,
           company_name: companyName,
        renewal_cost: 10,
         sub_end: '2026-12-31'
     }
   };
   const res = makeRes();

   await addSubscription(req, res);

   const [rows] = await pool.query(
          'SELECT * FROM subscriptions WHERE user_id = ? AND company_name = ?',
            [userId, companyName]
        );

        const passed = rows.length > 0;

  printTest(
       '3. Subscription Insert Validation',
    'Valid data should pass validation and proceed to database insertion',
        passed ? 'Subscription insert persisted successfully in database' : "Subscription insert works but doesn't successfully update into database",
        passed
      );
    } catch (error) {
      printTest(
      '3. Subscription Insert Validation',
     'Valid data should pass validation and proceed to database insertion',
            error.message,
            false
        );
    } finally {
        await removeUserData(userId);
    }
}

async function loginCredentialTest() {
    let userId;

 try {
    const user = await createUser('login');
       userId = user.userId;

      if (!user.ok) {
         return printTest(
           '4. Login Credential Check',
             'Matching credentials should allow login access',
               'User registration did not complete successfully',
                false
            );
        }
 const req = {
     body: {
       email: user.email,          password: user.password
            }
        };
        const res = makeRes();

        await login(req, res);

        const passed = res.statusCode === 200 && res.body?.success === true;

      printTest(
        '4. Login Credential Check',
        'Matching credentials should allow login access',
         passed ? 'Correct credentials successfully authenticated user' : 'Correct credentials did not authenticate user',
         passed
   );
    } catch (error) {
   printTest(
     '4. Login Credential Check',
   'Matching credentials should allow login access',
       error.message,
            false
        );
  } finally {
        await removeUserData(userId);
    }
}

async function runTests()
 {
  console.log('Unit Testing ');

    await passwordHashTest();
    await totalSpendingTest();
    await subscriptionInsertTest();
    await loginCredentialTest();

    await pool.end();
}

runTests().catch(async (error) => 
    {
    console.error(error.message);
    try {
        await pool.end();
    } catch {
        // Ignore shutdown errors
    }
    process.exit(1);
});
