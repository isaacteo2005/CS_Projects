# Sprint 1 Report (3/1/2025)

# Demo Video
https://youtu.be/bymkeiEpBpo
## What's New (User Facing)
 * Registration page
 * Login page

## Work Summary (Developer Facing)
Since this is the first sprint, we started by getting everything set up so we downloaded node js and mySQL and set them up for the project. We then created the issues then split up the work. Isaac did the register page and Mauricio worked on the login page. We did run into an issue with the database where we weren't sure if we should use a shared database or if we should each have our own database. We ended up deciding on creating our own seperate databases each based on the schema provided in the repo and if we needed to provide a shared database in the final product, we'll figure it out once we get there. We also had to spend a lot of time learning how to create and understand javascript and html code as we both primarily coded in C++ before starting this project.

## Unfinished Work
We still have not finished adding and deleting subscriptions, analytics page, and dashboard UI. We plan on completeing these issues by the next sprint.

## Completed Issues/User Stories
Here are links to the issues that we completed in this sprint:

 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/10)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/7)
 
 ## Incomplete Issues/User Stories
 Here are links to issues we worked on but did not complete in this sprint:
 
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/12) <<We did not get to this issue because we plan on finishing it next sprint.>
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/11) <<<We did not get to this issue because we plan on finishing it next sprint.>>
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/9) <<<We did not get to this issue because we plan on finishing it next sprint.>
 * https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/8 <We did not get to this issue because we plan on finishing it next sprint. >
 
 Examples of explanations (Remove this section when you save the file):
  * "We ran into a complication we did not anticipate (explain briefly)." 
  * "We decided that the feature did not add sufficient value for us to work on it in this sprint (explain briefly)."
  * "We could not reproduce the bug" (explain briefly).
  * "We did not get to this issue because..." (explain briefly)

## Code Files for Review
Please review the following code files, which were actively developed during this sprint, for quality:
 * [subController]https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/controllers/subController.js
 * [userController]https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/controllers/userController.js
 * [subscription]https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/routes/subscriptions.js
 * [auth] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/routes/auth.js
 * [database] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/config/database.js
 * [server] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/server.js
 * [Register] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/Register.js
 * [Register.css] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/Register.css
 * [login] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/login.js
 * [api] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/services/api.js
 * [App] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/App.js
 * [App.css] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/App.css
## Retrospective Summary
Here's what went well:
  * we were able to figure out how to set up the database, frontend, and backend correctly
  * our meetings have been consistent and we are able to do it every week
  * we were able to finish what we assigned ourselves to do for the first sprint
 
Here's what we'd like to improve:
   * figure out how our database is gonna work in the final product
   * figure out how to implement into an android instead of a site
   * sharing progress with each other more often
  
Here are changes we plan to implement in the next sprint:
   * dashboard UI, insert and delete functionality, analytics page
   * maybe 2FA if we have time