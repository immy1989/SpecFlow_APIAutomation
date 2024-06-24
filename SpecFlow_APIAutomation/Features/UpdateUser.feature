Feature: Update User API

A short summary of the feature

@Regression 
Scenario: Update user information - [PUT Update]
	Given the API endpoint for updating user information
	When I send a PUT request for user ID "morpheus" with updated job "zion resident"
	Then the user information should be successfully updated with the job "zion resident"
	And the response code should be 200 
