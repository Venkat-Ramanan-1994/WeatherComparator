using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System.Reflection;

namespace WeatherComparator.pageFactory.WeatherPage
{
    public class WeatherPage
    {
        IWebDriver driver { get; set; }
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public WeatherPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
        }

        [FindsBy(How = How.XPath, Using = "//input[@id='searchBox']")]
        public IWebElement searchBoxCityDropdown { get; set; }

        public IWebElement checkBoxCityDropdown(string cityName)
        {
            IWebElement obj = driver.FindElement(By.XPath("//label[contains(text(),'" + cityName + "')]//input"));
            return obj;
        }

        public IWebElement citySectionInMap(string cityName)
        {
            IWebElement obj = driver.FindElement(By.XPath("//div[@title='" + cityName + "']"));
            return obj;
        }

        public IWebElement temperatureOfCityInTheMap(string cityName)
        {
            IWebElement obj = driver.FindElement(By.XPath("//div[@title='" + cityName + "']//span[@class='tempRedText']"));
            return obj;
        }

        [FindsBy(How = How.XPath, Using = "//b[contains(text(),'Temp in Degrees')]")]
        public IWebElement temperatureInWeatherDetailsPopup { get; set; }


    }
}
