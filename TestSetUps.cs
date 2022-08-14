using AutomationTest;
using AutomationTest.commonutils;
using AutomationTest.reporting;
using AutomationTest.reporting.serilog;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
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
    public class TestSetUps
    {
        [OneTimeSetUp]
        public void TestSetup_OneTimeSetUp()
        {
            if (ReportProperties.ExtentReport == null)
            {
                ReportProperties.ResultsFullPath = new Util().CreateResultsFolder(TestProperties.ReportPath, ReportProperties.ReportFolderName);
                ExtentHtmlReporter htmlReporter = new ExtentHtmlReporter(ReportProperties.ResultsFullPath + @"\ExtentReport.html");
                ReportProperties.ExtentReport = new ExtentReports();
                ReportProperties.ExtentReport.AttachReporter(htmlReporter);
            }
        }

        [OneTimeTearDown]
        public void TestSetUp_OneTimeTearDown()
        {
            ReportProperties.ExtentReport.Flush();
        }
    }
}
