using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Configuration;
using TechTalk.SpecFlow;
using WeatherComparator.pageFactory.HomePage;
using WeatherComparator.pageFactory.WeatherPage;
using WeatherComparator.stepDefinitions.Utilities;

namespace WeatherComparator.stepDefinitions.WeatherTest_NDTV_Web_Steps
{
    [Binding]
    public sealed class Get_Weather_Data_From_NDTV_Web_Steps
    {

        IWebDriver driver;
        private WebUtils objWebUtils;
        private HomePage objHome;
        private WeatherPage objWeather;
        public int defaultTimeOutSeconds;

        public Get_Weather_Data_From_NDTV_Web_Steps(IWebDriver Driver)
        {
            driver = Driver;
            objWebUtils = new WebUtils(driver);
            objHome = new HomePage(driver);
            objWeather = new WeatherPage(driver);
            defaultTimeOutSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["defaultTimeOutSeconds"]);
        }

        [Given(@"I open the website's home page")]
        public void GivenIOpenTheWebsiteHomePage()
        {
            driver.Navigate().GoToUrl($"https://www.ndtv.com/");
            objWebUtils.WaitForPageLoading(defaultTimeOutSeconds);
        }

        [When(@"I navigate to the weather section of the website")]
        public void WhenINavigateToTheWeatherSectionOfTheWebsite()
        {
            objWebUtils.itemClick(objHome.linkNoThanks);
            objWebUtils.JavaScriptClick(objHome.buttonExpandOptionsHeaderSection);
            objWebUtils.JavaScriptClick(objHome.linkWeatherHeaderSection);
        }

        [When(@"I select a city: '(.*)' using Pin Your City from the left")]
        public void WhenISelectACityUsingPinYourCityFromTheLeft(string cityName)
        {
            objWebUtils.WaitForPageLoading(defaultTimeOutSeconds);
            objWebUtils.WaitForElementToLoad(objWeather.searchBoxCityDropdown, defaultTimeOutSeconds);
            objWeather.searchBoxCityDropdown.SendKeys(cityName);
            objWebUtils.itemClick(objWeather.checkBoxCityDropdown(cityName));
        }

        [Then(@"I should see that corresponding city '(.*)' is available on the map with temperature information")]
        public void ThenIShouldSeeThatCorrespondingCityIsAvailableOnTheMapWithTemperatureInformation(string cityName)
        {
            objWebUtils.WaitForElementToLoad(objWeather.citySectionInMap(cityName), defaultTimeOutSeconds);
            Assert.IsTrue(objWebUtils.IsElementDisplayed(objWeather.citySectionInMap(cityName)), $"The selected city : {cityName} is displayed in the Map");
        }

        [When(@"I select the city '(.*)' on the map")]
        public void WhenISelectTheCityOnTheMap(string cityName)
        {
            objWebUtils.JavaScriptClick(objWeather.citySectionInMap(cityName));
        }

        [Then(@"I should see the corresponding city's '(.*)' weather details")]
        public void ThenIShouldSeeTheCorrespondingWeatherDetails(string cityName)
        {
            string temperatureOfTheCity = objWeather.temperatureInWeatherDetailsPopup.GetAttribute("innerText").Trim();
            Assert.IsTrue(objWebUtils.IsElementDisplayed(objWeather.temperatureInWeatherDetailsPopup), $"The selected city : {cityName}'s weather : {temperatureOfTheCity} is displayed in the Map");
            Console.WriteLine($"The selected city : {cityName}'s weather/temp in Celcius : {temperatureOfTheCity} is displayed in the Map");
        }


    }
}
