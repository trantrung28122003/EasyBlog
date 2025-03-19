using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace EasyBlog.SharedLibrary.Logs
{
    public static class LogException
    {
        public static void LogExceptions(Exception ex)
        {
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);

        }

        private static void LogToFile(string message) => Log.Information(message);

        private static void LogToConsole(string message) => Log.Warning(message);

        private static void LogToDebugger(string message) => Log.Debug(message);
    }
}
