@regression @comparison
Feature: Compare_Weather_Data_From_NDTVWeb_And_API
	In order to avoid failures
	As an automation tester
	I want to compare the temperature data between NDTV web and API along with a variance

Background:
	Given  Load the request data for Get Weather API from json file 'GetWeatherAPIRequest.json'

@web @phase_1 @tc_002
Scenario Outline: Validate if corresponding city is available on the map & validate the weather details on clicking the city
Given   Load scenario outline description '<Description>'
Given   I open the website's home page
When    I navigate to the weather section of the website
And     I select a city: '<City>' using Pin Your City from the left
Then    I should see that corresponding city '<City>' is available on the map with temperature information
When    I select the city '<City>' on the map
Then    I should see the corresponding city's '<City>' weather details
Examples: 
| Description           | City      |
| NDTVWeatherCoimbatore | Ahmedabad |

@api @phase_2 @tc_002
Scenario Outline: Validate if Get Weather API returns valid response for the corresponding city
Given   Load scenario outline description '<Description>'
Given   I set Get Weather service API endpoint
When    I set request Header for the API Type - '<requestType>' with request Paramater '<requestParam>'
And     I send Get HTTP Request
Then    I validate the HTTP Response
Examples: 
| Description             | requestType	| requestParam |
| APIByCityNameAhmedabad  | ByCityName	| Ahmedabad    |


@phase_3 @tc_001
Scenario Outline: Validate if the temperature from web and API lies within a variance of 2 degree celsius
Given   Load scenario outline description '<Description>'
Given   I retrieve the data from two weather objects: Object1-Web and Object2-API for the city '<City>'
Then    I compare the temperature using a variance logic 
Examples: 
| Description                | City      |
| WeatherComparisonAhmedabad | Ahmedabad |
	