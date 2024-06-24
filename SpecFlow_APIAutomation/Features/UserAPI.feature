Feature: User API Testing


@Smoke
Scenario: Retrieve User Information - [GET Single User]
	Given a valid user id 
	And I request information for the user with id "2"
	When the response should contain the user details:
	     | FirstName   | LastName      |
         | Janet       | Weaver        |
	Then the response status code should be 200

@Smoke
Scenario: Add a New User - [POST Create]
	Given a new user with the following details:
	      | Name        | Job           |
          | morpheus    | leader        |
	When I add the new user via API
	Then the response status code should return 201
	And  the response should contain the newly added user details

@Smoke
Scenario: Successful API Login - [POST Successful]
	Given Given a user with valid credentials:
	      | Email                 | Password      |
          | eve.holt@reqres.in    | cityslicka    |
	When the user sends a POST request to the login endpoint with the credentials
	Then the API should respond with a 200 OK status code
	And  the response should contain a valid access token

@Smoke
Scenario: UnSuccessful API Login - [POST UnSuccessful]
	Given Given a user with missing credentials:
	      | Email                 |
          | peter@klaven          |
	When the user sends a POST request to the login endpoint with the missing credentials
	Then the API should respond with a 400 BadRequest status code
	And  the response should contain a error as "Missing password"

