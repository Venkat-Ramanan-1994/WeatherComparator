@regression
Feature: Get_Weather_Data_From_API
	In order to avoid failures
	As an automation tester
	I want to verify the API Response for the Get Weather API and build weather Object 2 for comparison

Background:
	Given  Load the request data for Get Weather API from json file 'GetWeatherAPIRequest.json'

@api @phase_2 @tc_001
Scenario Outline: Validate if Get Weather API returns valid response for the corresponding city
Given   Load scenario outline description '<Description>'
Given   I set Get Weather service API endpoint
When    I set request Header for the API Type - '<requestType>' with request Paramater '<requestParam>'
And     I send Get HTTP Request
Then    I validate the HTTP Response
Examples: 
| Description                | requestType			   | requestParam |
| APIByCityNameCoimbatore    | ByCityName			   | Coimbatore	  |
| APIByCityId                | ByCityId                | 1279233      |
| APIByGeographicCoordinates | ByGeographicCoordinates | 76.97, 11    |
| APIByZipCode               | ByZipCode			   | 641001,IN    |
| APIByCityNameAhmedabad     | ByCityName			   | Ahmedabad	  |
