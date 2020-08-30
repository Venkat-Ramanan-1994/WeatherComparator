using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Newtonsoft.Json;
using RestSharp;


namespace WeatherComparator.stepDefinitions.Utilities
{
    public class APIUtils
    {
        public APIUtils()
        {
            requestHeaders = new Dictionary<string, string>();
        }
        private static string apiEndpointURL { get; set; }
        private static Dictionary<string, string> requestHeaders { get; set; }

        public void SetAPIEndpointURL(string requestURL)
        {
            apiEndpointURL = requestURL;
            Console.WriteLine($"API End Point URL: {apiEndpointURL}");
        }

        public void SetRequestHeaders(string requestJSON)
        {
            requestHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestJSON);
        }

        public IRestResponse CallRestService(Method methodType)
        {
            RestClient client = new RestClient(apiEndpointURL);
            RestRequest request = new RestRequest(methodType);

            if (requestHeaders.Count != 0)
            {               
                foreach (var header in requestHeaders)
                    if (header.Key != "testCaseKey")
                    {
                        Console.WriteLine($"API Request Headers: {header.Key} -- {header.Value}");
                        request.AddParameter(header.Key, header.Value);
                    }
            }

            string apiKey = ConfigurationManager.AppSettings["apiKey"];            
            request.AddParameter("appid", apiKey);
            
            IRestResponse response = client.Execute(request);                 
                       
            return response;
        }
    }
}
