﻿using AutomationTest.commonutils;
using AutomationTest.reporting;
using AutomationTest.reporting.serilog;
using AventStack.ExtentReports;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.uiautomation
{
    
    public class WebDriver
    {
        public IWebDriver SeleniumDriver;
        private ExtentTest ExtentTest;
        public IJavaScriptExecutor JSRunner => SeleniumDriver as IJavaScriptExecutor;
        public String Title => SeleniumDriver.Title;
        protected ILogger logger = LoggerConfig.Logger;
        private ImageUtil screenshotManager = new ImageUtil();
        Util util = new Util();

        public String BrowserType { get; private set; }

        internal WebDriver(ExtentTest extentTest)
        {
            ExtentTest = extentTest;
        }

        public WebElement FindElement(Locator locator)
        {
            logger.Here().Information("Finding element: " + locator.Xpath);
            return new WebElement() { webElement = SeleniumDriver.FindElement(locator.By), locator = locator };
        }

        public List<WebElement> FindElements(Locator locator)
        {
            logger.Here().Information("Finding all elements: " + locator.Xpath);
            var webElements = SeleniumDriver.FindElements(locator.By);
            List<WebElement> result = new();
            int count = 1;
            foreach(var element in webElements)
            {
                result.Add(new WebElement()
                {
                    webElement = element,
                    locator = new Locator() 
                    { 
                        By = locator.By, 
                        Xpath = $"({locator.Xpath})[{count}]" }
                });
                count++;
            }
            return result;
        }

        public void Navigate(String url)
        {
            if(SeleniumDriver == null)
            {
                SeleniumDriver = OpenBrowser();
            }
            ExtentTest.Log(Status.Info, "Navigating to Url: " + url);
            SeleniumDriver.Navigate().GoToUrl(url);
        }

        private IWebDriver OpenBrowser()
        {
            BrowserType = TestProperties.Browser;
            logger.Here().Information("Opening Browser: " + BrowserType);
            switch (BrowserType.ToLower().Trim())
            {
                case "chrome":
                    ChromeOptions chromeOptions = new()
                    {
                        AcceptInsecureCertificates = true
                    };
                    chromeOptions.AddArgument("--start-maximized");
                    //chromeOptions.AddArgument("--disable-popup-blocking");
                    chromeOptions.AddArgument("--ignore-certificate-errors");
                    chromeOptions.AddArgument("--incognito");
                    return new ChromeDriver(TestProperties.DriverPath, chromeOptions);
                case "firefox":
                    FirefoxOptions firefoxOptions = new()
                    {
                        AcceptInsecureCertificates = true
                    };
                    firefoxOptions.AddArgument("--start-maximized");
                    //firefoxOptions.AddArgument("--disable-popup-blocking");
                    firefoxOptions.AddArgument("--ignore-certificate-errors");
                    firefoxOptions.AddArgument("--private");
                    return new FirefoxDriver(TestProperties.DriverPath, firefoxOptions);
                case "edge" or "msedge":
                    EdgeOptions edgeOptions = new()
                    {
                        AcceptInsecureCertificates = true,
                        UseInPrivateBrowsing = true
                    };
                    IWebDriver driver = new EdgeDriver(TestProperties.DriverPath, edgeOptions);
                    driver.Manage().Window.Maximize();
                    return driver;
            }
            throw new Exception("Browser not supported: " + BrowserType);
        }

        public void Quit()
        {
            logger.Here().Information("Quiting Driver");
            if(SeleniumDriver != null)
            {
                SeleniumDriver.Quit();
            }
        }

        public Object ExecuteJavaScript(String script)
        {
            var results = JSRunner.ExecuteScript(script);
            ExtentTest.Log(Status.Info, $"Executed Javascript: '{script}'");
            logger.Here().Information($"Executed Javascript: '{script}'" );
            return results;
        }

        public String GetScreenshotPath(bool TakeFullScreenshot = false, String ImageFormat = "png", String ScreenshotType = "")
        {
            String ScreenshotPath = util.CreateDirectory(ReportProperties.ResultsFullPath, ReportProperties.ScreenshotFolderName) + "\\" + ExtentTest.Model.Name + Util.getCurrentTime() + "." + ImageFormat;
            try
            {
                if (TakeFullScreenshot)
                {
                    screenshotManager.GetEntireScreenshot(ScreenshotPath, SeleniumDriver);
                }
                else
                {
                    screenshotManager.CaptureScreenshot(ScreenshotPath, SeleniumDriver, ScreenshotType);
                }
                return ScreenshotPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
        }

        public void ClickJS(WebElement webElement)
        {
            JSRunner.ExecuteScript("arguments[0].click();", webElement.webElement);
            logger.Here().Information($"Click Element '{webElement.locator.Xpath}' using JavaScript");
        }

    }
}
