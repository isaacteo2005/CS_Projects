function printResult(name, passed, details) {
    console.log(`\n${name}`);
    console.log(`Result: ${passed ? 'Pass' : 'Fail'}`);
    console.log(`Details: ${details}`);
}

async function sendJson(baseUrl, path, options = {}) {
    const response = await fetch(`${baseUrl}${path}`, {
        headers: { 'Content-Type': 'application/json' },
        ...options,
    });

    const text = await response.text();
    const body = text ? JSON.parse(text) : null;

    return { response, body };
}

async function cleanupTestUser(pool, userId) {
    if (!userId) return;

    await pool.query('DELETE FROM subscriptions WHERE user_id = ?', [userId]);
    await pool.query('DELETE FROM user_stats WHERE user_id = ?', [userId]);
    await pool.query('DELETE FROM users WHERE user_id = ?', [userId]);
}

async function run() {
    process.env.NODE_ENV = 'test';

    const { default: app } = await import('../server.js');
    const { default: pool } = await import('../config/database.js');

    const server = app.listen(0);
    const { port } = server.address();
    const baseUrl = `http://127.0.0.1:${port}`;

    const email = `integration.${Date.now()}@test.com`;
    const password = 'IntegrationPassword123!';
    let userId;
    let subscriptionId;

    try {
        const { response: healthResponse, body: healthBody } = await sendJson(
            baseUrl,
            '/test'
        );
        printResult(
            '1. Health Route Integration Test',
            healthResponse.status === 200 &&
                healthBody.message === 'Server is working!',
            `Status ${healthResponse.status}, message: ${healthBody.message}`
        );

        const { response: registerResponse, body: registerBody } = await sendJson(
            baseUrl,
            '/api/auth/register',
            {
                method: 'POST',
                body: JSON.stringify({
                    name: 'Integration User',
                    email,
                    password,
                }),
            }
        );
        userId = registerBody?.userId;
        printResult(
            '2. Register Route Integration Test',
            registerResponse.status === 200 &&
                registerBody?.success === true &&
                Boolean(userId),
            `Status ${registerResponse.status}, userId: ${userId ?? 'missing'}`
        );

        const { response: loginResponse, body: loginBody } = await sendJson(
            baseUrl,
            '/api/auth/login',
            {
                method: 'POST',
                body: JSON.stringify({ email, password }),
            }
        );
        printResult(
            '3. Login Route Integration Test',
            loginResponse.status === 200 &&
                loginBody?.success === true &&
                loginBody?.user?.email === email,
            `Status ${loginResponse.status}, authenticated user: ${loginBody?.user?.email ?? 'missing'}`
        );

        const { response: statsResponse, body: statsBody } = await sendJson(
            baseUrl,
            `/api/user-stats/${userId}`
        );
        printResult(
            '4. Get User Stats Integration Test',
            statsResponse.status === 200 &&
                Number(statsBody?.user_id) === Number(userId),
            `Status ${statsResponse.status}, overall_spent: ${statsBody?.overall_spent}`
        );

        const { response: addSubscriptionResponse, body: addSubscriptionBody } =
            await sendJson(baseUrl, '/api/subscriptions', {
                method: 'POST',
                body: JSON.stringify({
                    user_id: userId,
                    company_name: 'Netflix Integration',
                    renewal_cost: 15.99,
                    sub_end: '2026-12-31',
                }),
            });
        subscriptionId = addSubscriptionBody?.subscription_id;
        printResult(
            '5. Add Subscription Integration Test',
            addSubscriptionResponse.status === 200 &&
                addSubscriptionBody?.success === true &&
                Boolean(subscriptionId),
            `Status ${addSubscriptionResponse.status}, subscriptionId: ${subscriptionId ?? 'missing'}`
        );

        const { response: listSubscriptionsResponse, body: listSubscriptionsBody } =
            await sendJson(baseUrl, `/api/subscriptions/user/${userId}`);
        const addedSubscription = Array.isArray(listSubscriptionsBody)
            ? listSubscriptionsBody.find(
                  (subscription) =>
                      Number(subscription.subscription_id) === Number(subscriptionId)
              )
            : null;
        printResult(
            '6. Get Subscriptions Integration Test',
            listSubscriptionsResponse.status === 200 && Boolean(addedSubscription),
            `Status ${listSubscriptionsResponse.status}, subscriptions found: ${
                Array.isArray(listSubscriptionsBody) ? listSubscriptionsBody.length : 0
            }`
        );

        const { response: updateSubscriptionResponse, body: updateSubscriptionBody } =
            await sendJson(baseUrl, `/api/subscriptions/${subscriptionId}`, {
                method: 'PUT',
                body: JSON.stringify({
                    company_name: 'Netflix Premium Integration',
                    renewal_cost: 19.99,
                    ongoing: true,
                }),
            });
        printResult(
            '7. Update Subscription Integration Test',
            updateSubscriptionResponse.status === 200 &&
                updateSubscriptionBody?.success === true,
            `Status ${updateSubscriptionResponse.status}, success: ${updateSubscriptionBody?.success}`
        );

        const { response: updateStatsResponse, body: updateStatsBody } = await sendJson(
            baseUrl,
            `/api/user-stats/${userId}`,
            {
                method: 'PUT',
                body: JSON.stringify({
                    overall_spent: 19.99,
                    last_login: '2026-03-25',
                    session_count: 5,
                }),
            }
        );
        printResult(
            '8. Update User Stats Integration Test',
            updateStatsResponse.status === 200 &&
                updateStatsBody?.success === true,
            `Status ${updateStatsResponse.status}, message: ${updateStatsBody?.message}`
        );

        const { response: deleteSubscriptionResponse, body: deleteSubscriptionBody } =
            await sendJson(baseUrl, `/api/subscriptions/${subscriptionId}`, {
                method: 'DELETE',
            });
        printResult(
            '9. Delete Subscription Integration Test',
            deleteSubscriptionResponse.status === 200 &&
                deleteSubscriptionBody?.success === true,
            `Status ${deleteSubscriptionResponse.status}, success: ${deleteSubscriptionBody?.success}`
        );
    } finally {
        await cleanupTestUser(pool, userId);
        await new Promise((resolve) => server.close(resolve));
        await pool.end();
    }
}

run().catch((error) => {
    console.error('\nIntegration quick test failed:', error.message);
    process.exit(1);
});
