using AutomationTest.commonutils;
using AutomationTest.reporting;
using AutomationTest.reporting.serilog;
using AventStack.ExtentReports;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AutomationTest.uiautomation
{
    
    public class WebDriver
    {
        public IWebDriver SeleniumDriver;
        private WebDriverWait Wait;
        private readonly ExtentTest ExtentTest;
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
            return new WebElement() { webElement = SeleniumDriver.FindElement(locator.By), locator = locator, executor = JSRunner };
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
                        Xpath = $"({locator.Xpath})[{count}]", 
                    },
                    executor = JSRunner
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
            if(SeleniumDriver != null)
            {
                logger.Here().Information("Quiting Driver");
                SeleniumDriver.Quit();
            }
        }

        public Object ExecuteJavaScript(String script)
        {
            var results = JSRunner.ExecuteScript(script);
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



        public void WaitForPageLoad(int timeOut = 60)
        {
            Wait = new WebDriverWait(SeleniumDriver, new TimeSpan(0, 0, timeOut));
            Wait.Until((d) =>
            {
                try
                {
                    return ExecuteJavaScript("return document.readyState").Equals("complete");
                }
                catch (InvalidOperationException e)
                {
                    //Window is no longer available
                    return e.Message.ToLower().Contains("unable to Driver browser");
                }
                catch (WebDriverException e)
                {
                    //Browser is no longer available
                    return e.Message.ToLower().Contains("unable to connect");
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public ITargetLocator SwitchTo() => SeleniumDriver.SwitchTo();

        public String CurrentWindowHandle => SeleniumDriver.CurrentWindowHandle;
        public String PageSource => SeleniumDriver.PageSource;

        public IOptions Manage() => SeleniumDriver.Manage();

        public Navigation Navigate()
        {
            return new Navigation(SeleniumDriver.Navigate(), this.Url);
        }
        public IReadOnlyCollection<String> WindowHandles => SeleniumDriver.WindowHandles;

        public String Url => SeleniumDriver.Url;

        public void Close()
        {
            SeleniumDriver.Close();
        }

        public class Navigation {

            protected ILogger logger = LoggerConfig.Logger;

            private INavigation navigation;
            private String Url;

            internal Navigation (INavigation navigation, String Url)
            {
                this.navigation = navigation;
                this.Url = Url;
            }
            public void Back()
            {
                logger.Here().Information($"Navigating Back from '{Url}'");
                navigation.Back();   
            } 
            public void Forward()
            {
                logger.Here().Information($"Navigating Forward from '{Url}'");
                navigation.Forward();   
            }
            
            public void Reload()
            {
                logger.Here().Information($"Reloading page '{Url}'");
                navigation.Refresh();   
            }

        }

        public void SetImplicitWait(double seconds)
        {
            SeleniumDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
        }

        public void SetPageLoadTimeOut(double seconds)
        {
            SeleniumDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(seconds);
        }
        public IWindow CurrentWindow => SeleniumDriver.Manage().Window;

        public void WaitOnCondition(Object data, WaitConditions conditions, bool conditionState, int timeOut = 20)
        {
            Wait = new WebDriverWait(SeleniumDriver, TimeSpan.FromSeconds(timeOut));

            switch (conditions)
            {
                case WaitConditions.TextPresent:
                    logger.Here().Information($"Waiting for condtion({nameof(WaitConditions.TextPresent)} {data}) to be {conditionState}, for {timeOut} seconds");
                    if (conditionState)
                    {
                        Wait.Until(ExpectedConditions.ElementIsVisible(Locator.PartialText((string)data).By));
                    }
                    else
                    {
                        Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(Locator.PartialText((string)data).By));
                    }
                    break;
                case WaitConditions.ElementPresent:
                    if (data.GetType() == typeof(Locator))
                    {
                        logger.Here().Information($"Waiting for condtion({nameof(WaitConditions.ElementPresent)} {((Locator)data).By}) to be {conditionState}, for {timeOut} seconds");
                        Wait.Until((d) =>
                        {
                            bool isPresent = d.FindElements(((Locator)data).By).Count != 0;
                            return isPresent == conditionState;
                        });
                    }
                    else if (data.GetType() == typeof(WebElement))
                    {
                        Wait.Until((d) =>
                        {
                            var element = ((WebElement)data).webElement;
                            bool isPresent = element != null && element.Displayed;
                            return isPresent == conditionState;
                        });
                    }
                    break;
                case WaitConditions.ElementDisplayed:
                    logger.Here().Information($"Waiting for condtion({WaitConditions.ElementDisplayed} [Element : {((WebElement)data).locator.By}]) to be {conditionState}, for {timeOut} seconds");
                    if (conditionState)
                    {
                        Wait.Until(ExpectedConditions.ElementIsVisible(((Locator)data).By));
                    }
                    else
                    {
                        Wait.Until(ExpectedConditions.InvisibilityOfElementLocated(((Locator)data).By));
                    }
                    break;
                default:
                    break;
            }

        }

        public void Pause(int Milliseconds)
        {
            logger.Here().Information($"Pausing Execution for {Milliseconds} milliseconds");
            Thread.Sleep(Milliseconds);
        }
    }
}
