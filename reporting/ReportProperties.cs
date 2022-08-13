using AventStack.ExtentReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.reporting
{
    internal class ReportProperties
    {
        internal static String ReportFolderName = "\\TestResults";
        internal static String ScreenshotFolderName = "\\Screenshots";

        internal static String ResultsFullPath = "";

        internal static ExtentReports ExtentReport;
    }
}
