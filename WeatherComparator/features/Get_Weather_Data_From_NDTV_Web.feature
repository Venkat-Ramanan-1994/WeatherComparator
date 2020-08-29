@regression
Feature: Get_Weather_Data_From_NDTV_Web
	In order to avoid failures
	As an automation tester
	I want to verify the "Pin your city" functionality in NDTV's weather section and build weather Object 1 for comparison


@web @phase_1 @tc_001
Scenario Outline: Validate if corresponding city is available on the map & validate the weather details on clicking the city
Given   Load scenario outline description '<Description>'
Given   I open the website's home page
When    I navigate to the weather section of the website
And     I select a city: '<City>' using Pin Your City from the left
Then    I should see that corresponding city '<City>' is available on the map with temperature information
When    I select the city '<City>' on the map
Then    I should see the corresponding city's '<City>' weather details
Examples: 
| Description           | City        |
| NDTVWeatherCoimbatore | Coimbatore  |
| NDTVWeatherAhmedabad  | Ahmedabad   |
