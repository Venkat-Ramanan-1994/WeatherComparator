using log4net;
using System;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using log4net.Config;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System.Diagnostics;
using System.Drawing;
using BoDi;

namespace WeatherComparator.stepDefinitions.Utilities
{
    [Binding]
    public class Hooks
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string featureTitle;
        private readonly string[] featureTags;
        private readonly string featureDescription;
        private readonly string[] scenarioTags;
        private readonly string scenarioTitle;
        private int defaultTimeOutSeconds;
        private static Stopwatch stepduration;
        private readonly IObjectContainer _objectContainer;
        public IWebDriver driver;
        public ICapabilities capabilities;
        private WebUtils objBrowserUtils;

        public Hooks(IObjectContainer objectContainer)
        {
            XmlConfigurator.Configure();
            featureTags = FeatureContext.Current.FeatureInfo.Tags;
            featureDescription = FeatureContext.Current.FeatureInfo.Description;
            scenarioTitle = ScenarioContext.Current.ScenarioInfo.Title;
            scenarioTags = ScenarioContext.Current.ScenarioInfo.Tags;
            _objectContainer = objectContainer;
            objBrowserUtils = new WebUtils(driver);
        }
        [BeforeFeature()]
        public static void BeforeFeatureBegins()
        {
            featureTitle = FeatureContext.Current.FeatureInfo.Title;
            logger.Debug("***** Begin Feature: " + featureTitle + " *****");
            Console.WriteLine($"[Begin Feature] - {featureTitle}");

            var _browser = ConfigurationManager.AppSettings["defaultBrowser"].ToLower();
            string _env = ConfigurationManager.AppSettings["env"].ToLower();
            logger.Debug("***** Testing Environment: " + _env + " *****");
            Console.WriteLine($"Testing Environment: {_env}");
            logger.Debug("***** Browser: " + _browser + " *****");
            Console.WriteLine($"Browser: {_browser}");

        }
        [BeforeScenario]
        public void BeforeScenario()
        {
            var browser = ConfigurationManager.AppSettings["defaultBrowser"].ToLower();
            //    var mode = ConfigurationManager.AppSettings["seleniumMode"].ToLower();

            Console.WriteLine("[BeforeScenario] - Start " + browser);
            logger.Debug("***** Begin Scenario: " + scenarioTitle + " **");
            Console.WriteLine("Scenario: " + scenarioTitle);

            switch (browser)
            {
                case "chrome":
                    StartChrome();
                    break;
                case "firefox":
                    StartFF();
                    break;
                case "ie":
                    StartIE();
                    break;
                case "safari":
                    //TODO Safari
                    break;

                default:
                    Console.WriteLine($"Invalid Browser Name Configured: {browser}");
                    StartChrome();
                    break;
            }



        }

        [BeforeStep]
        public void BeforeStepBegins()
        {
            logger.Debug("***** " + ScenarioContext.Current.StepContext.StepInfo.StepDefinitionType + ": " + ScenarioContext.Current.StepContext.StepInfo.Text);
            stepduration = Stopwatch.StartNew();

        }

        [AfterStep]
        public void AfterStepEnds()
        {
            stepduration.Stop();
            string scenarioDesc = ScenarioContext.Current.ScenarioInfo.Title;
            scenarioDesc = ScenarioContext.Current.ScenarioInfo.Title + ", " + ScenarioContext.Current["scenarioOutlineDesc"];
            logger.Debug($"Step Execution Time : {stepduration.Elapsed}");

        }

        [AfterScenario]
        public void AfterScenario()
        {
            var browser = ConfigurationManager.AppSettings["defaultBrowser"].ToLower();
            logger.Debug("***** End Scenario: " + scenarioTitle + " **");
            Console.WriteLine("[AfterScenario] - Quit browser");
            bool testPass = true;

            if (ScenarioContext.Current.TestError != null)
            {

                string errMsg = "error: " + ScenarioContext.Current.TestError.Message;
                logger.Debug(errMsg);
                Console.WriteLine(errMsg);
                //driver.Manage().Cookies.AddCookie(cookieTestFail);
                testPass = false;
                try
                {
                    objBrowserUtils.TakeScreenshot(driver);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Screenshot error: " + ex);
                    objBrowserUtils.TakeScreenshot();

                }
            }

            if (browser.Equals("chrome"))
            {
                closeDriver();
                KillProcByName("chromedriver");
            }
            else if (browser.Equals("firefox"))
            {
                closeDriver();
                KillProcByName("firefox");
            }
            else if (browser.Equals("ie"))
            {
                closeDriver();
                KillProcByName("internetexplorer");
            }


        }

        [AfterFeature]
        public static void AfterFeature()
        {
            logger.Debug("***** End Feature: " + featureTitle + " *****");
            Console.WriteLine("[End Feature] - " + featureTitle);
        }

        private void StartChrome(Boolean localHub = false)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--start-maximized");
            options.AddArgument("--test-type");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-infobars");
            options.AddArguments("chrome.switches", "--disable-extensions");
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);


            //var browserDriverConfig = (NameValueCollection)ConfigurationManager.GetSection("browserDriverConfig");
            //var chromeDriverDirectory = browserDriverConfig["chromeDriverDirectory"].ToLower();
            //string exactPath = System.IO.Path.GetFullPath(chromeDriverDirectory);
            defaultTimeOutSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["defaultTimeOutSeconds"]);
            driver = new ChromeDriver(options);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            var agent = js.ExecuteScript("return navigator.userAgent");
            Console.WriteLine("userAgent = " + agent);
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(defaultTimeOutSeconds));
            writeBrowserInfoToConsole(driver);
            ScenarioContext.Current["mainWindowHandle"] = driver.WindowHandles.FirstOrDefault();
            _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
        }

        private void StartFF()
        {

            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            // service.FirefoxBinaryPath = @ffExe;
            service.HideCommandPromptWindow = true;
            service.SuppressInitialDiagnosticInformation = true;
            FirefoxOptions options = new FirefoxOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--start-maximized");
            options.AddArgument("--test-type");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-infobars");
            driver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(30));
            capabilities = ((RemoteWebDriver)driver).Capabilities;
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
            driver.Manage().Window.Maximize();
            ScenarioContext.Current["mainWindowHandle"] = driver.WindowHandles.FirstOrDefault();
            _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
            writeBrowserInfoToConsole(driver);
        }

        private void StartIE()
        {
            var options = new InternetExplorerOptions();
            options.IntroduceInstabilityByIgnoringProtectedModeSettings = true;
            options.EnsureCleanSession = true;
            driver = new InternetExplorerDriver(options);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
            capabilities = ((RemoteWebDriver)driver).Capabilities;
            driver.Manage().Window.Maximize();
            ScenarioContext.Current["mainWindowHandle"] = driver.WindowHandles.FirstOrDefault();
            _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
            writeBrowserInfoToConsole(driver);
        }

        public void KillProcByName(string processName)
        {
            Process[] localAll = Process.GetProcesses();
            try
            {
                foreach (Process proc in Process.GetProcessesByName(processName))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                logger.Debug(ex.Message);
            }
        }

        public void closeDriver()
        {
            try
            {
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(0);
                //driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie("zaleniumTestPassed", testPass.ToString()));
            }
            catch (NullReferenceException npe)
            {
                Console.WriteLine("NullReferenceException: The HTTP request to the remote WebDriver server may have timedout during initialization.", Color.Red);
                Console.WriteLine(npe.ToString(), Color.Red);
            }
            catch (WebDriverException wde)
            {

                Console.WriteLine("WebDriverException: The HTTP request to the remote WebDriver server for URL timed out after defaultTimeSeconds", Color.Red);
                Console.WriteLine(wde.ToString(), Color.Red);
            }
            finally
            {
                driver.Dispose();
                driver.Quit();
            }
        }

        public void writeBrowserInfoToConsole(IWebDriver driver)
        {
            capabilities = ((RemoteWebDriver)driver).Capabilities;
            var browserName = capabilities.GetCapability("browserName");
            var browserVersion = capabilities.GetCapability("browserVersion");
            Console.WriteLine("Browser Name: " + browserName);
            Console.WriteLine("Browser Version: " + browserVersion);
        }

    }
}
