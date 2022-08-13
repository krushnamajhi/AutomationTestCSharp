using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.reporting.serilog
{
    public class LoggerConfig
    {
        public static readonly String FileName = "../../../Logs/Log_.log";

        static readonly String outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.ffff} [Thread:{ThreadNumber}] | {Level:u3} | [Line:{LineNumber}] | {ClassName}.{Method}( '{Message}' ) {NewLine}";

        static LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
/*
        //Logs to File
        public readonly static ILogger Logger =
           new LoggerConfiguration().MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.File(FileName,
                outputTemplate: outputTemplate,
                rollingInterval: RollingInterval.Hour, retainedFileCountLimit: 70).CreateLogger();
*/
        //Logs to Console
        public readonly static ILogger Logger  = 
           new LoggerConfiguration().MinimumLevel.ControlledBy(levelSwitch).WriteTo.Console(
               outputTemplate: outputTemplate).CreateLogger();
    }
}
