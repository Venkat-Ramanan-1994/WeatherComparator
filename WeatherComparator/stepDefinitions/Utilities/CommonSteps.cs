using log4net;
using OpenQA.Selenium;
using System.Reflection;
using TechTalk.SpecFlow;

namespace WeatherComparator.stepDefinitions.Utilities
{
    [Binding]
    public class CommonSteps
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        IWebDriver driver;

        public CommonSteps(IWebDriver Driver)
        {
            driver = Driver;
        }


        [Given(@"Load scenario outline description '(.*)'")]
        public void GivenLoadScenarioOutlineDescitpion(string desc)
        {
            ScenarioContext.Current["scenarioOutlineDesc"] = desc;
        }

    }
}
