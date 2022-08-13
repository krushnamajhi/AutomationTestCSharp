using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.reporting.serilog
{
    public static class LoggerExtensions
    {
		public static ILogger Here(this ILogger logger, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			return logger.ForContext("ClassName", new StackTrace().GetFrame(1).GetMethod().ReflectedType)
				.ForContext("LineNumber", sourceLineNumber)
				.ForContext("Method",memberName)
				.ForContext("ThreadNumber", System.Threading.Thread.CurrentThread.ManagedThreadId);
		}
	}
}
