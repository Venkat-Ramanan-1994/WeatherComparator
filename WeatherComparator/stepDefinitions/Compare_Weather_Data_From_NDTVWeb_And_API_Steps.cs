using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using TechTalk.SpecFlow;

namespace WeatherComparator.stepDefinitions
{
    [Binding]
    public sealed class Compare_Weather_Data_From_NDTVWeb_And_API_Steps
    {
        private double temperatureFromWeb;
        private double temperatureFromAPI;

        public Compare_Weather_Data_From_NDTVWeb_And_API_Steps()
        {
            
        }

        [Given("I retrieve the data from two weather objects: Object1-Web and Object2-API for the city '(.*)'")]
        public void GivenIRetrieveTheDataFromTwoWeatherObjectsObject1WeAndObject2API(string city)
        {
            temperatureFromWeb = Convert.ToDouble(FeatureContext.Current["temperatureOfTheCityFromWeb"].ToString());
            Console.WriteLine($"Temperature of the city: {city} and Data from the web : {temperatureFromWeb}");
            temperatureFromAPI = Convert.ToDouble(FeatureContext.Current["temperatureOfTheCityFromAPI"].ToString());
            Console.WriteLine($"Temperature of the city: {city} and Data from the API : {temperatureFromAPI}");
        }

        [Then("I compare the temperature using a variance logic")]
        public void ThenTheTwoNumbersAreAdded()
        {
            double acceptedVariance = Convert.ToDouble(ConfigurationManager.AppSettings["AcceptedTemperatureVariance"]);
            Console.WriteLine($"Accepted Variance in Temperature : {acceptedVariance}");
            double actualVariance = temperatureFromWeb - temperatureFromAPI;
            Console.WriteLine($"Actual Variance in Temperature : {actualVariance}");
            Assert.IsTrue(Math.Abs(actualVariance) <= acceptedVariance);
        }
    }
}
