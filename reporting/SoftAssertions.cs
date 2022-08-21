using AutomationTest.commonutils;
using AutomationTest.exceptions;
using AutomationTest.reporting.serilog;
using AventStack.ExtentReports;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AutomationTest.reporting
{
    public class SoftAssertions
    {
        TestPackage TestPackage;
        internal SoftAssertions(TestPackage TestPackage)
        {
            this.TestPackage = TestPackage;
        }
        private readonly List<SingleAssert>
            _verifications = new List<SingleAssert>();

        private void Add(string message, string expected, string actual)
        {
            _verifications.Add(new SingleAssert(message, expected, actual, TestPackage));
        }

        public void AreEqual(string message, object expected, object actual)
        {
            Add(message, JsonSerializer.Serialize(expected), JsonSerializer.Serialize(actual));
        }

        public void True(string message, bool actual)
        {
            _verifications
                .Add(new SingleAssert(message, true.ToString(), actual.ToString(), TestPackage));
        }



        public void AssertAll()
        {
            var failed = _verifications.Where(v => v.Failed).ToList();
            failed.Should().BeEmpty();
        }

        class SingleAssert
        {
            private readonly string _message;
            private readonly string _expected;
            private readonly string _actual;
            public readonly TestCaseException _exception;

            public bool Failed { get; }

            public SingleAssert(string message, string expected, string actual, TestPackage TestPackage)
            {
                _message = message;
                _expected = expected;
                _actual = actual;
                Failed = _expected != _actual;

                try
                {
                    Failed.Should().BeFalse();
                    TestPackage.ExtentTest.Log(Status.Pass, message + " => " + expected);
                    LoggerConfig.Logger.Here().Information(message);
                }
                catch (Exception e)
                {
                    TestContext.CurrentContext.Result.Assertions.Append(new AssertionResult(AssertionStatus.Failed, message, Environment.StackTrace));
                    if (TestPackage.Driver.SeleniumDriver != null)
                    {
                        ImageUtil imageUtil = new();
                        TestPackage.ExtentTest.Log(Status.Fail, this.ToString(), imageUtil.getImageMedia(TestPackage.Driver.GetScreenshotPath()));
                    }
                    else
                    {
                        TestPackage.ExtentTest.Log(Status.Fail, this.ToString());
                    }
                    _exception = new TestCaseException("Assertion Error => " + this.ToString(), e, TestPackage.ExtentTest);
                }
            }

            public override string ToString()
            {
                return $"'{_message}' assert was expected to be " +
                        $"<{_expected}> but was <{_actual}>";
            }
        }
    }
}
