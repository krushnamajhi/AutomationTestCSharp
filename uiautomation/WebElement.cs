using AutomationTest.reporting.serilog;
using NUnit.Framework;
using OpenQA.Selenium;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.uiautomation
{
    public class WebElement
    {
        internal IWebElement webElement;
        internal Locator locator;
        protected ILogger logger = LoggerConfig.Logger;

        internal WebElement()
        {

        }

        public WebElement SendKeys(String text)
        {
            logger.Here().Information($"Entering text '{text}' in element : '{locator.Xpath}'");
            webElement.SendKeys(text);
            return this;
        }

        public WebElement PressKey(String key)
        {
            logger.Here().Information($"Pressing Key '{key}' in element : '{locator.Xpath}'");
            webElement.SendKeys(key);
            return this;
        }
        public WebElement EnterText(String text, bool clearBefore = true)
        {
            if (clearBefore)
            {
                webElement.Clear();
            }
            return SendKeys(text);
        }

        public void Click()
        {
            logger.Here().Information($"Clicking Element: '{locator.Xpath}'");
            webElement.Click();
        }

        public WebElement FindElement(Locator locator)
        {
            return new WebElement()
            {
                webElement = webElement.FindElement(locator.By),
                locator = new Locator()
                {
                    Xpath = this.locator.Xpath + locator.Xpath,
                    By = locator.By
                }
            };
        }

        public String GetAttributeValue(String attribute)
        {
            logger.Here().Information($"Getting attribute value of '{attribute}' of element '{locator.Xpath}'");
            String value = webElement.GetAttribute(attribute);
            logger.Here().Information($"Retreived Attribute value of '{attribute}' of element '{locator.Xpath}' is '{value}'");
            return value;
        }
    }
}
