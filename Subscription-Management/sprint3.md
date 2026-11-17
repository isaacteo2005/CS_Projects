# Sprint 3 Report (4/22/2026 - 5/1/2026)

## What's New (User Facing)
 * cloud database implementation (supabase)
 * most expensive subscription shown on analytics page
 * recently deleted subscription shown on analytics page
 * fixed subscription price input only accepting whole numbers
 * analytics total spent not displaying correctly fixed

## Work Summary (Developer Facing)
Most of the work done this sprint were more focused finishing work outside of the issues like the final writing assignment and prepping and presenting the class presentation. For the developer side, this sprint was more focused on making last touches for the class presentation and fixing any bugs that arised. The most expensive/recently deleted subscritpions were added as finishing touches for the analytics page, this gives the user additional information they can view on that page. We also reconfigured our backend code to connect to supabase instead of mySQL in order to upload data to the cloud. Lastly, a bug arised where subscirtion prices only accepted whole numbers so we fixed it so that it can now add cents as well and total spent on the analytics page now shows correctly.

## Unfinished Work
The work that we haven't finished yet were stretch goals. These were the 2FA and start/end dates for each subscription. We were more focused on other tasks outside of working on the project mentioned in the work summary which is why we didn't work on these stretch goals.

## Completed Issues/User Stories
Here are links to the issues that we completed in this sprint:

 * total spent bug fix: https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/38
 * cloud database: https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/31
 * recently deleted/most expensive subscription: https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/42
 * subscription price input bug fix: https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/41
 
 ## Incomplete Issues/User Stories
 Here are links to issues we worked on but did not complete in this sprint:
 
 * https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/32  We did not get to this issue as it was a stretch goal that didn't have high priority
 * https://github.com/WSU-CPTS322-SP26/Subscription-Management/issues/30 We did not get to this issue as it was a stretch goal that didn't have high priority

## Code Files for Review
Please review the following code files, which were actively developed during this sprint, for quality:
 * [subController](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/controllers/subController.js)
 * [userController] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/controllers/userController.js
 * [userStatsController] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/controllers/userStatsController.js
 * [supabase](https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/backend/config/supabase.js)
 * [Analytics] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/Analytics.js
 * [Dashboard] https://github.com/WSU-CPTS322-SP26/Subscription-Management/blob/main/subscription-tracker/frontend/src/components/Dashboard.js
 
## Retrospective Summary
Here's what went well:
  * We were able to fix some bugs making the application a lot more cleaner
  * Attendance as great as each of was were always availible for meeting every week
  * communication went well as we often communicated our progress over discord
  * Analytics page now worked properly with additional features
 
Here's what we'd like to improve:
   * some issues/fixes weren't properly documented so we'd like to work on that
   * our application could use some design fixes like moving the account delete to be not next to logout, more testing of the design would be needed
   * not many issues were created this sprint so creating more user stories could be useful
  
Here are changes we plan to implement in the next sprint:
   * work on the finishing and implementing stretch goal issues
   * work on ideas that came up after class presentation like a family account and a subscription cancel page