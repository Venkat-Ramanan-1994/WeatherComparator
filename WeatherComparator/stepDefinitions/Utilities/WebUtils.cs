using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace WeatherComparator.stepDefinitions.Utilities
{
    public class WebUtils
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected IWebDriver driver { get; set; }
        public WebUtils(IWebDriver Driver)
        {
            driver = Driver;
        }

        public void itemClick(IWebElement element, int timeOut = 30)
        {
            WaitForElementToLoad(element, timeOut);
            try
            {
                element.Click();
            }
            catch (Exception ex)
            {
                // retrying for java.net.ConnectException: Connection refused
                Console.WriteLine("Received " + ex.Message + ". Retrying itemClick().");
                Thread.Sleep(1000);
                element.Click();
            }

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeOut));
            wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
        }

        public void JavaScriptClick(IWebElement element, int timeOut = 30)
        {
            WaitForElementToLoad(element, timeOut);
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            executor.ExecuteScript("arguments[0].click();", element);
        }

        public IList<string> GetAvailableOptionsFromDropdown(IWebElement element)
        {
            IList<string> availableOptions = new List<string>();
            var selectElement = new SelectElement(element);
            IList<IWebElement> options = selectElement.Options;
            foreach (IWebElement option in options)
            {
                availableOptions.Add(option.GetAttribute("innerText"));
            }
            return availableOptions;
        }


        public string WaitForPageLoading(int Timeout)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Timeout));
                wait.Until<bool>((d) =>
                {
                    try
                    {
                        string test = ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").ToString();
                        if (test.Equals("complete"))
                            return true;
                        return false;
                    }
                    catch
                    {
                        return false;
                    }
                });
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public bool WaitForElementToLoad(IWebElement element, int maxWaitTime = 180)
        {
            bool isElementLoaded = false;
            int waitTime = 0;
            while (waitTime < maxWaitTime)
            {
                if (IsElementDisplayedAndEnabled(element))
                {
                    isElementLoaded = true;
                    break;
                }
                Thread.Sleep(1000);
                waitTime++;
            }
            return isElementLoaded;
        }

        public bool IsElementDisplayedAndEnabled(IWebElement element)
        {
            bool isEnabledAndDisplayed = false;
            try
            {
                isEnabledAndDisplayed = (element.Displayed && element.Enabled);
                return isEnabledAndDisplayed;
            }
            catch (Exception)
            {
                return isEnabledAndDisplayed;
            }
        }

        public bool IsElementDisplayed(IWebElement element)
        {
            bool isDisplayed = false;
            try
            {
                isDisplayed = (element.Displayed);
                return isDisplayed;
            }
            catch (Exception e)
            {
                return isDisplayed;
            }
        }

        public void TakeScreenshot(IWebDriver driver, bool wholePage = true)
        {
            var logsDir = ConfigurationManager.AppSettings["logsDir"] + "\\";
            logsDir = AppDomain.CurrentDomain.BaseDirectory + "\\" + logsDir;
            string saveLocation = @logsDir;
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-hhmm-ss-ffff");
            if (wholePage)
            {
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(saveLocation + timestamp + "_ViewPort.png", ScreenshotImageFormat.Png);
                Bitmap fullPageSS = TakeFullPageScreenshot(driver);
                fullPageSS.Save(saveLocation + timestamp + "_FullPage.png", ImageFormat.Png);

            }
            else
            {
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(saveLocation + timestamp + "_ViewPort.png", ScreenshotImageFormat.Png);
                Bitmap fullPageSS = TakeFullPageScreenshot(driver);
                fullPageSS.Save(saveLocation + timestamp + "_FullPage.png", ImageFormat.Png);
            }

            Console.WriteLine("View Port Screenshot: {0}", new Uri(saveLocation + timestamp + "_ViewPort.png"));
            Console.WriteLine("Full Page Screenshot: {0}", new Uri(saveLocation + timestamp + "_FullPage.png"));
        }

        public Bitmap TakeFullPageScreenshot(IWebDriver _driver)

        {
            Bitmap stitchedImage = null;
            try
            {
                long totalwidth1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.offsetWidth");//documentElement.scrollWidth");
                long totalHeight1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return  document.body.parentNode.scrollHeight");
                int totalWidth = (int)totalwidth1;
                int totalHeight = (int)totalHeight1;
                // Get the Size of the Viewport
                long viewportWidth1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return document.body.clientWidth");//documentElement.scrollWidth");
                long viewportHeight1 = (long)((IJavaScriptExecutor)_driver).ExecuteScript("return window.innerHeight");//documentElement.scrollWidth");
                int viewportWidth = (int)viewportWidth1;
                int viewportHeight = (int)viewportHeight1;
                // Split the Screen in multiple Rectangles
                List<Rectangle> rectangles = new List<Rectangle>();
                // Loop until the Total Height is reached
                for (int i = 0; i < totalHeight; i += viewportHeight)
                {
                    int newHeight = viewportHeight;
                    // Fix if the Height of the Element is too big
                    if (i + viewportHeight > totalHeight)
                    {
                        newHeight = totalHeight - i;
                    }
                    // Loop until the Total Width is reached
                    for (int ii = 0; ii < totalWidth; ii += viewportWidth)
                    {
                        int newWidth = viewportWidth;
                        // Fix if the Width of the Element is too big
                        if (ii + viewportWidth > totalWidth)
                        {
                            newWidth = totalWidth - ii;
                        }
                        // Create and add the Rectangle
                        Rectangle currRect = new Rectangle(ii, i, newWidth, newHeight);
                        rectangles.Add(currRect);
                    }
                }

                // Build the Image
                stitchedImage = new Bitmap(totalWidth, totalHeight);
                // Get all Screenshots and stitch them together
                Rectangle previous = Rectangle.Empty;
                foreach (var rectangle in rectangles)
                {
                    // Calculate the Scrolling (if needed)
                    if (previous != Rectangle.Empty)
                    {
                        int xDiff = rectangle.Right - previous.Right;
                        int yDiff = rectangle.Bottom - previous.Bottom;
                        // Scroll
                        //selenium.RunScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        ((IJavaScriptExecutor)_driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                        System.Threading.Thread.Sleep(200);
                    }
                    // Take Screenshot
                    var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                    // Build an Image out of the Screenshot
                    Image screenshotImage;
                    using (MemoryStream memStream = new MemoryStream(screenshot.AsByteArray))
                    {
                        screenshotImage = Image.FromStream(memStream);
                    }
                    // Calculate the Source Rectangle
                    Rectangle sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);
                    // Copy the Image
                    using (Graphics g = Graphics.FromImage(stitchedImage))
                    {
                        g.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                    }
                    // Set the Previous Rectangle
                    previous = rectangle;
                }
            }
            catch (Exception ex)
            {
                // handle
            }
            return stitchedImage;
        }

        public void TakeScreenshot()
        {
            var logsDir = ConfigurationManager.AppSettings["logsDir"] + "\\";
            logsDir = AppDomain.CurrentDomain.BaseDirectory + "\\" + logsDir;
            Bitmap memoryImage;
            //Set full width, height for image
            memoryImage = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                   Screen.PrimaryScreen.Bounds.Height,
                                   PixelFormat.Format32bppArgb);
            Size s = new Size(memoryImage.Width, memoryImage.Height);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            memoryGraphics.CopyFromScreen(0, 0, 0, 0, s);
            string str = "";

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-hhmm-ss");
            string saveLocation = @logsDir;
            str = saveLocation + timestamp + ".png";
            memoryImage.Save(str);
            Console.WriteLine("Screenshot: {0}", new Uri(str));
        }


    }
}
