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
    public class TestBase : Base
    {
        protected SoftAssertions SoftAssert;

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
            SoftAssert.AssertAll();
        }
    }
}
