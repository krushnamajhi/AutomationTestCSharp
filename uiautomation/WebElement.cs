using AutomationTest.exceptions;
using AutomationTest.reporting.serilog;
using AventStack.ExtentReports;
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
        internal IJavaScriptExecutor executor;
        internal ExtentTest ExtentTest;

        internal WebElement()
        {

        }

        public WebElement SendKeys(String text)
        {
            try
            {
                logger.Here().Information($"Entering text '{text}' in element : '{locator.Xpath}'");
                webElement.SendKeys(text);
                return this;
            }
            catch(Exception e)
            {
                throw new TestCaseException($"Unable to Enter Text in the field {locator.Xpath}", e, ExtentTest);
            }
        }

        public WebElement PressKey(String key)
        {
            try
            {
                logger.Here().Information($"Pressing Key '{key}' in element : '{locator.Xpath}'");
                webElement.SendKeys(key);
                return this;
            }
            catch(Exception e)
            {
                throw new TestCaseException($"Unable to Press key on the field {locator.Xpath}", e, ExtentTest);
            }
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
                },
                executor = executor,
                ExtentTest = ExtentTest
            };
        }

        public String GetAttribute(String attribute)
        {
            String value = webElement.GetAttribute(attribute);
            logger.Here().Information($"Retreived Attribute value of '{attribute}' of element '{locator.Xpath}' is '{value}'");
            return value;
        }

        public void Clear()
        {
            webElement.Clear();
            logger.Here().Information($"Cleared input Field '{locator.Xpath}'");
        }

        public bool Displayed()
        {
            bool displayed = webElement.Displayed;
            logger.Here().Information($"Element '{locator.Xpath}' is Displayed => '{displayed}'");
            return displayed;

        } 
        public bool Enabled()
        {
            bool enabled = webElement.Enabled;
            logger.Here().Information($"Element '{locator.Xpath}' is Enabled => '{enabled}'");
            return enabled;
        }
        
        public bool Selected()
        {
            bool selected = webElement.Selected;
            logger.Here().Information($"Element '{locator.Xpath}' is Selected: '{selected}'");
            return selected;
        }
        public String TagName()
        {
            String TagName = webElement.TagName;
            logger.Here().Information($"TagName of the Element '{locator.Xpath}' is '{TagName}'");
            return TagName;
        }
        
        public String Text()
        {
            String text = webElement.Text;
            logger.Here().Information($"Text retrieved from the Element '{locator.Xpath}' is '{text}'");
            return text;
        }

        public void ClickJS()
        {
            try
            {
                executor.ExecuteScript("arguments[0].click();", webElement);
                logger.Here().Information($"Clicked Element '{locator.Xpath}' using JavaScript");
            }
            catch(Exception e)
            {
                new TestCaseException($"Failed to Click element '{locator.Xpath}' from javascript",e, ExtentTest);
            }
        }
    }
}
