using AutomationTest.commonutils;
using AutomationTest.reporting;
using AutomationTest.reporting.serilog;
using AutomationTest.uiautomation;
using AventStack.ExtentReports;
using NUnit.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest
{
    public class TestBase : TestSetUps
    {
        protected WebDriver Driver => TestPackage.Driver;
        protected ExtentTest ExtentTest => TestPackage.ExtentTest;
        protected SoftAssertions SoftAssert;

        [SetUp]
        public void TestBaseSetUp()
        {
            TestPackages.Add(TestContext.CurrentContext.Test.ID, new TestPackage());
            SoftAssert = new SoftAssertions(TestPackage);
        }

        [TearDown]
        public void TestTearDown()
        {
            Driver.Quit();
            if (TestPackages.ContainsKey(TestContext.CurrentContext.Test.ID))
            {
                TestPackages.Remove(TestContext.CurrentContext.Test.ID);
            }
            SoftAssert.AssertAll();
        }
    }
}
