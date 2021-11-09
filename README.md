Battleship v1
=============


This is the INX Battleship test solution.


# Battleship Calibration Challenge

The engineers at a shipyard would like your assistance in building several new Battleships. As a gun battery is fitted,
 a rotational calibration needs to be performed to ensure the battery is moving as expected. The batteries are currently
 moved manually, however the engineers would like an application that can perform this automatically. Your task is to build
 this application.

The application stores the instructions for 5 turrets at a time. Each turret will rotate in the sequence provided to it.
 The rotation always starts with the battery pointing on the port side of the ship (min 0 degrees) and moving to the starboard
 side (max 180 degrees).

The application has two modes of operations. The first mode is “settings” where the parameters for the calibration are defined.
 The second mode is “run” which runs the sequence and reports on the actions which were taken by the device.


## Application Type

The part of the application you submit depends on the role you are applying for.

- **Back-end**: The application you submit will be a back-end Web API Service
- **Full-stack**: The application will be a web application which communicates with a back-end API.
- **Front-end**: The application will be a front-end web application which will communicate with a provided web API service. Swagger documentation will be provided for the API Endpoints.

Parts of this document are annotated with specific requirements for the specific roles.

### API Specification

The back-end web API will have the following routes:

- **PUT /api/calibration/settings** – used for updating the settings – how you store the settings is unimportant.
- **POST /api/calibration/run** – used for running the program based on the settings provided and reporting the result.

The /api/settings route takes the following input:

- Sequence – The order in which the turrets will be rotated. A turret can be rotated more than once. The sequence is an array.
- An array of `Turrets`, which includes:
  - Caliber – The battery caliber in mm, with a minimum of 102mm and a maximum of 450mm.
  - Location – The location of the turret on the ship. This is an enum with the possible values of “Bow”, and “Stern”.
  - Rotation start point – The rotation start in degrees. This value must be between 0 and 180.
  - Rotation end point – The rotation end point in degrees. This value must be between 0 and 180 and must be more than
     the start point.
- The /api/calibration/settings response will be an indication whether the settings were accepted or not.
- The /api/calibration/run route does not take any inputs.
- The /api/calibration/run response will be a report how many times each turret was tested and the total distance rotated in
   degrees.

An example JSON response could look like the following, where the int is the turret tested:


### Design

The front-end web application will have 3 screens:

- Settings screen which has inputs for “Sequence” and the related turret settings.
- For front-end developers only:
  - After the settings have been submitted, it should show a preview of the turrets rotating on the ship (this can be as
     simple or fancy as you like). This will give the engineer a chance to make any corrections before continuing.
  - After the run command is submitted it should show a confirmation screen saying that the sequence has been run
     successfully.
- For full-stack developers only:
  - After the settings have been submitted and accepted a screen with the “Run” buttons should be shown to the user.
  - After the run command has been submitted, the front-end should display the result returned from the server in an
     easy-to-understand way.

### Considerations

- This engineering firm has strict auditing requirements, and thus the API should have appropriate logging.
- Both the front-end and API should have automated tests that assert the application is working as expected.
- The calibration cannot run with no settings applied.
- There is no limit to the length of the test sequence.
- A turret might have settings applied but not be included in the sequence.
   `{ "0": { "timesTested": 1, "rotated": 130 }, "1": { "timesTested": 1, "rotated": 130 },
   "2": { "timesTested": 2, "rotated": 260 } }`
- The back-end technology should be .NET, Java or Node.js based.
- For the Web API format, pick either JSON or GraphQL.
- The front-end should be built in Vue.js, React or Angular 2+, and must use Typescript. You’re welcome to
   consume any NPM packages you’d find helpful.

### Deliverables

- The source code, any test data and tests.
- Instructions on how to run your solution, including any dependencies which could be required.
- Documentation including any assumptions, things which were omitted by your solution, why you chose the
   technology you used, and what you could do to improve your solution

### Time to complete task

Depending on the application type, we anticipate this task should take under 4 hours to complete. We ask
 that you return this challenge in 7 days.
