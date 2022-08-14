using AutomationTest.reporting.serilog;
using AutomationTest.uiautomation;
using AventStack.ExtentReports;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest
{
    public class Base
    {
        protected ILogger logger = LoggerConfig.Logger;

        protected TestPackage TestPackage;
        protected WebDriver Driver => TestPackage.Driver;

        protected ExtentTest ExtentTest => TestPackage.ExtentTest;

    }
}
