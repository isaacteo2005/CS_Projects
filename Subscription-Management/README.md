# Subscription-Management
Subscription Management App
## Project summary
### One-sentence description of the project
App that manages subscriptions for users with useful statistics.
### Additional information about the project
Due to the rise of more subscription services, it has become much harder to track what you are subscribed too. This app allows the user to add subscriptions to a list and track how much they have spent on these subscriptions as well as give useful reminders on when a trial/subscription is ending. This would help in managing finances in regards to subscription.
## Installation
### Prerequisites

Downloads
download node js: https://nodejs.org/
download mysql (556 M) https://dev.mysql.com/downloads/installer/
download git https://git-scm.com/install/
npm (comes with node js)

Run each in a terminal to check if installation was successful:
node --version
npm --version
mysql --version
git --version
### Add-ons
Backend Addons:
Express.js: Web framework for handling API requests
mysql: MySQL database driver
cors: cross-origin requests from React frontend
dotenv: environment variables for sensitive data

Frontend Addons:
React: UI library for building UI
Axios: Handles HTTP requests to the backend API

### Installation Steps
1. clone repo: git clone https://github.com/yourusername/subscription-tracker.git
cd subscription-tracker

2. set up database
# Log into MySQL
mysql -u root -p

# Run the schema file to create tables
source database/schema.sql;

# Exit MySQL
exit;
3. set up backend
cd backend
npm install
cp .env.example .env
# Open .env in a text editor and update:DB_PASSWORD=yourpassword

4. set up frontend
cd frontend
npm install
5. start application
First way(Test the server):
1. head to Subscription-Management\subscription-tracker\backend: type command: node server.js
2. head to http://localhost:3001/test
Message should be:{"message":"Server is working!"}

Second way(Start the server):
open backend and frontend folder in terminal
backend terminal: npm run dev
frontend terminal: npm start
Open browser in: http://localhost:3000
## Functionality
Usage (Once the app becomes finished)
1. Click Register
Enter name, email, password then create account (adds account to the user table and a stats record to user_stats)
2. Login
Enter username and password then click login (system verifies credentials, last_login updated in user_stats, session_count += 1)
3. Adding a subscription
Click add a subscription, fill in details, click save subscription and it should appear in your dashboard (adds subscription to subscription table)
4. Deleting subscriptions
Click Delete, mark subscriptions to delete with checkboxes, click delete and confirmation and all marked subscriptions should be deleted
5. Viewing subscription details
Clicking on a subscription should bring down a dropmenu that displays the stats of the subscription
6. Viewing user statistics
There would be an individual side menu to click on that would show the overall stats for the user
7. Dashboard UI
Displays all user subscriptions in a clean interface, It allows quick acess to subcription details, It serves as the main page to manage subscriptions
8. Analytics Page
Provides insights into total spending, Displays statistics related to subscriptions, Helps users better understand their finacial spendings

## Known Problems
No known problems are currently documented by the team at this time. 

## Contributing
TODO: Leave the steps below if you want others to contribute to your project.
1. Follow installation
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -m "new feature"`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request
## Additional Documentation
- [Sprint 1 Report](./sprint1.md)
