using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System.Reflection;

namespace WeatherComparator.pageFactory.HomePage
{
    public class HomePage
    {
        IWebDriver driver { get; set; }
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HomePage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.XPath, Using = "//a[contains(text(),'No Thanks')]")]
        public IWebElement linkNoThanks { get; set; }


        [FindsBy(How = How.CssSelector, Using = "a#h_sub_menu")]
        public IWebElement buttonExpandOptionsHeaderSection { get; set; }


        [FindsBy(How = How.XPath, Using = "//a[contains(text(),'WEATHER')]")]
        public IWebElement linkWeatherHeaderSection { get; set; }



    }
}
