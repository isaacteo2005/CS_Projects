ALTER TABLE subscriptions
    ALTER COLUMN renewal_cost TYPE numeric(10,2),
    ALTER COLUMN renewal_cost SET DEFAULT 0.00,
    ALTER COLUMN total_spent TYPE numeric(10,2),
    ALTER COLUMN total_spent SET DEFAULT 0.00;

ALTER TABLE user_stats
    ALTER COLUMN overall_spent TYPE numeric(10,2),
    ALTER COLUMN overall_spent SET DEFAULT 0.00;

-- I added this into a query to the database to have more precise information about the costs of subscriptions.  