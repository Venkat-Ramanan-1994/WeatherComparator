using Castle.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using TechTalk.SpecFlow;
using TechTalk.SpecRun.Common.Helper;
using WeatherComparator.stepDefinitions.Utilities;
using AutomationException = WeatherComparator.stepDefinitions.Utilities.AutomationException;

namespace WeatherComparator.stepDefinitions
{
    [Binding]
    public sealed class Get_Weather_Data_From_API_Steps
    {
        private APIUtils objAPIUtils; 
        private dynamic requestJSONData;
        private IRestResponse restResponse;
        private readonly string[] featureTags;

        public Get_Weather_Data_From_API_Steps()
        {
            objAPIUtils = new APIUtils();
            featureTags = FeatureContext.Current.FeatureInfo.Tags;
        }

        [Given(@"I set Get Weather service API endpoint")]
        public void GivenISetGetWeatherServiceAPIEndpoint()
        {
            string apiEndPointURL = ConfigurationManager.AppSettings["apiEndPointURL"];
            objAPIUtils.SetAPIEndpointURL(apiEndPointURL);
        }

        [Given(@"Load the request data for Get Weather API from json file '(.*)'")]
        public void GivenLoadTheLoginDataFromJson(string fileName)
        {
            var dataDir = ConfigurationManager.AppSettings["dataDir"];
            Console.WriteLine($"Data Directory: {dataDir}");

            string json = File.ReadAllText(@dataDir + "/" + fileName);
            requestJSONData = JsonConvert.DeserializeObject(json);
            Console.WriteLine($"Json Data : {requestJSONData}");
        }

        [When(@"I set request Header for the API Type - '(.*)' with request Paramater '(.*)'")]
        public void WhenISetRequestHeaderForTheAPITypeWithRequestParamater(string requestType, string requestParam)
        {
            string requestHeader = null;            
            foreach (var requestJSON in requestJSONData)
            {
               if(requestJSON.testCaseKey == requestType)
                {                   
                    switch (requestType)
                    {
                        case "ByCityName":
                            if(requestJSON.q == requestParam)                            
                                requestHeader = Convert.ToString(requestJSON);                            
                            break;
                        case "ByCityId":
                            if (requestJSON.id == requestParam)                            
                                requestHeader = Convert.ToString(requestJSON);
                            break;
                        case "ByGeographicCoordinates":
                            if (requestParam.Contains(Convert.ToString(requestJSON.lat)))
                                requestHeader = Convert.ToString(requestJSON);
                            break;
                        case "ByZipCode":
                            if (requestJSON.zip == requestParam)
                                requestHeader = Convert.ToString(requestJSON);
                            break;
                    }
                }
            }
            if(requestHeader.IsNotNullOrEmpty())
                objAPIUtils.SetRequestHeaders(requestHeader);
            else
            {
                throw new AutomationException($"Unknown Request Paramater : {requestParam} , Check Test Data in the respective Json File");
            }
                
        }

        [When(@"I send Get HTTP Request")]
        public void WhenISendGetHTTPRequest()
        {
            restResponse = objAPIUtils.CallRestService(Method.GET);
        }

        [Then(@"I validate the HTTP Response")]
        public void ThenIValidateTheHTTPResponse()
        {
            Console.WriteLine($"Request to URL: {restResponse.ResponseUri} and the StatusCode: {restResponse.StatusCode}");
            Assert.IsTrue(restResponse.StatusCode == HttpStatusCode.OK, $"API request is successfull");
            Console.WriteLine($"Response Content: {restResponse.Content}");
            var responseObject = JsonConvert.DeserializeObject<dynamic>(restResponse.Content);
            string temperatureOfTheCity = responseObject.main.temp;
            if (temperatureOfTheCity.IsNotNullOrEmpty())
            {
                double temperatureOfTheCityFromAPI = Convert.ToDouble(temperatureOfTheCity) - 273.15;
                Console.WriteLine($"Temperature Of The City From Weather API : {temperatureOfTheCityFromAPI}");
                if (!featureTags.IsNullOrEmpty() && featureTags.Contains("comparison"))
                {
                    FeatureContext.Current["temperatureOfTheCityFromAPI"] = temperatureOfTheCityFromAPI;
                }
            }
           
        }
    }
}
