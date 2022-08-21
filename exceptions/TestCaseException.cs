using AutomationTest.reporting.serilog;
using AventStack.ExtentReports;
using AventStack.ExtentReports.MarkupUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.exceptions
{
    public class TestCaseException : Exception
    {
        public TestCaseException(String msg, Exception ex, ExtentTest extentTest) : base(msg, ex)
        {
            String exceptionMsg = msg + "\n" + ex.Message + "\n"+ Environment.StackTrace;
            LoggerConfig.Logger.Here().Error(exceptionMsg);
            var m = MarkupHelper.CreateCodeBlock(exceptionMsg, CodeLanguage.Xml);
            extentTest.Log(Status.Error, m);
        }
    }
}
