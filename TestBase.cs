using AutomationTest.commonutils;
using AutomationTest.reporting;
using AutomationTest.reporting.serilog;
using AutomationTest.uiautomation;
using AventStack.ExtentReports;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest
{
    public class TestBase : Base
    {
        protected SoftAssertions SoftAssert;
        private ImageUtil imageUtil = new();

        [SetUp]
        public void TestBaseSetUp()
        {
            TestPackage = new TestPackage();
            SoftAssert = new SoftAssertions(TestPackage);
        }

        [TearDown]
        public void TestTearDown()
        {
            Driver.Quit();
            if(TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                logger.Here().Error(TestContext.CurrentContext.Result.Message + "\n" + TestContext.CurrentContext.Result.StackTrace);
                ExtentTest.Error(TestContext.CurrentContext.Result.Message + "\n" + TestContext.CurrentContext.Result.StackTrace, imageUtil.getImageMedia(TestPackage.Driver.GetScreenshotPath()));
            }
            SoftAssert.AssertAll();
        }
    }
}
