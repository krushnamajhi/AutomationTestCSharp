using AutomationTest.reporting;
using AutomationTest.uiautomation;
using AventStack.ExtentReports;
using NUnit.Framework;

namespace AutomationTest
{
    public class TestPackage
    {
        public WebDriver Driver;
        public ExtentTest ExtentTest;

        public TestPackage()
        {
            ExtentTest = ReportProperties.ExtentReport.CreateTest(TestContext.CurrentContext.Test.MethodName);
            ExtentTest.AssignCategory(TestContext.CurrentContext.Test.ClassName);
            ExtentTest.AssignCategory(TestProperties.Browser);
            Driver = new WebDriver(ExtentTest);
        }

    }
}
