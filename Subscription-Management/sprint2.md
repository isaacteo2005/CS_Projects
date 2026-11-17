 # Sprint 2 Report 3/31/2026
#Demo Video: https://www.youtube.com/watch?v=nIWTK3TOcs4
## What's New (User Facing)
* Dashboard now pulls subscription data from the backend
* Users can view analytics/statistics for their subscriptions
* Users can delete subscriptions with a confirmation step
* Users can delete their account from the dashboard
* Additional testing was added to improve reliability
## Work Summary (Developer Facing)
During this sprint, we focused on moving the project from basic login and registration into the main subscription management experience.
We worked on connecting the dashboard to the backend so subscription data could be displayed dynamically instead of staying static in the frontend. We also added analytics-related functionality
delete confirmation behavior, and account deletion so the app feels more complete from a user perspective. We also spent time improving our backend configuration and adding tests like
 performance-related testing so the project would be easier to maintain. A major part of this sprint was learning how to connect frontend components, backend routes, and the 
database together in a way that worked smothly and correctly.

## Unfinished Work
There is a couple Issues that are still up that are meant to serve as extended goals if we have the time to implement them

## Completed Issues/User Stories
Here are links to the issues that we completed in this sprint:
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/29)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/28)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/23)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/22)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/21)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/20)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/19)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/18)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/17)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/11)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/9)
 * (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/8)
  
## Incomplete Issues/User Stories
Here are links to issues we worked on but did not complete in this sprint:
* (https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/31)<< Not strictly necessary but will be good to have for next sprint>>
  
## Code Files for Review
Please review the following code files, which were actively developed during this sprint, for quality:
* [Dashboard.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/Dashboard.js)
* [Dashboard.css](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/Dashboard.css)
* [Analytics.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/Analytics.js)
* [ConfirmDialog.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/ConfirmDialog.js)
* [ConfirmDialog.css](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/ConfirmDialog.css)
* [api.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/services/api.js)
* [userController.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/controllers/userController.js)
* [server.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/server.js)
* [users.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/routes/users.js)
* [standards-tests.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/tests/standards-tests.js)
* [performance-tests.js](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/tests/performance-tests.js)


## Work Summary (Developer Facing)
During this sprint we focused on making the project feel more complete by connecting more of the frontend to the backend and improving the main subscription management features. We worked on syncing dashboard data with the backend
adding delete-related functionality and improving the overall structure of the app with more testing and configuration updates. One challenge we faced was making sure different parts of the system worked together correctly, especially when connecting frontend components, backend routes and the database. 
 
## Retrospective Summary
Here's what went well:
  * We made good progress on the core features beyond login and registration
  * We improved the connection between the frontend, backend, and database
  * We added more testing to help make the project more reliable
 
Here's what we'd like to improve:
  * We need to keep improving code organization as the project grows
  
Here are changes we plan to implement in the next sprint:
  * Finish any remaining incomplete features and polish the user experience
  * Improve analytics and dashboard behavior where needed
