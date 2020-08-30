using log4net;
using OpenQA.Selenium;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow;

namespace WeatherComparator.stepDefinitions.Utilities
{
    [Binding]
    public class CommonSteps
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);            

        [Given(@"Load scenario outline description '(.*)'")]
        public void GivenLoadScenarioOutlineDescitpion(string desc)
        {   
            ScenarioContext.Current["scenarioOutlineDesc"] = desc;
        }      

    }
}
