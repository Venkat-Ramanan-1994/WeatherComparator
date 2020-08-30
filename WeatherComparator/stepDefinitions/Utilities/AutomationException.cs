using log4net;
using OpenQA.Selenium;
using System;

namespace WeatherComparator.stepDefinitions.Utilities
{
    public class AutomationException : Exception
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AutomationException));
        public AutomationException(string sExceptionMessage) : base(sExceptionMessage)
        {

        }

    }
}
