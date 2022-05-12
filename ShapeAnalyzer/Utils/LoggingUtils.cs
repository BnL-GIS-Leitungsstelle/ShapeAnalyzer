using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;

namespace ShapeAnalyzer.Utils
{
    public static class LoggingUtils
    {
        public static string GetLogFilePath()
        {
            return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    nameof(ShapeAnalyzer),
                    "log.txt");
        }

        public static void Init()
        {
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget
            {
                Name = "Logfile",
                FileName = GetLogFilePath(),
                ArchiveAboveSize = 10000000,
                MaxArchiveFiles = 3
            };          
            
            config.AddTarget(fileTarget);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);

            LogManager.Configuration = config;
        }
    }
}
