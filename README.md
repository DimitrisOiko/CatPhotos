# CatPhotos
Prerequisite: Users pc should have docker installed
Caution: In order for this solution to work on your localhost, sql server should not be installed as it will interfere with the dockerized sql server the program tries to launch

Step 1: open an cmd on the root folder (CatPhotos) of the project and run the command: docker-compose build
Step 2: run the command: docker-compose up

You now have the application running.

Please find the postman collection of the project in the same root folder
Otherwise you can go to the swagger page via this url: http://localhost:5555/swagger/index.html

Note: Hangfire is configured to 0 retry policy.