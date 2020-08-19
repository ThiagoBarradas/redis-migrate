using RedisMigrate.Models;
using System;
using System.Text.RegularExpressions;

namespace RedisMigrate
{
    class Program
    {
        public static void Main(string[] args = null)
        {
            try
            {
                var config = RedisMigrateConfiguration.Create();
                DisplayHeader(config);

                var processor = new RedisMigrateProcessor(config);

                processor.Execute();

                    processor.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Program Exception:");
                Console.WriteLine(" - {0}\n\n{1}", e.Message, e.StackTrace);
            }
        }

        private static void DisplayHeader(RedisMigrateConfiguration config)
        {
            Logger.LogLineWithLevel("INFO", "RedisMigrate Application Started");
            Logger.LogLine("");
            Logger.LogLine("Configuration:");
            Logger.LogLine("- MaxThreads: {0}", config.MaxThreads);
            Logger.LogLine("- OriginConnectionString: {0}", Regex.Replace(config.OriginConnectionString, "(password=).*(\\,)", "password=*****,"));
            Logger.LogLine("- OriginDatabase: {0}", config.OriginDatabase);
            Logger.LogLine("- OriginFilter: {0}", config.OriginFilter);
            Logger.LogLine("- OriginPopulateEnabled: {0}", config.OriginPopulateEnabled);
            Logger.LogLine("- OriginPopulateQuantity: {0}", config.OriginPopulateQuantity);
            Logger.LogLine("- OriginPopulatePrefix: {0}", config.OriginPopulatePrefix);
            Logger.LogLine("- DestinationConnectionString: {0}", Regex.Replace(config.DestinationConnectionString, "(password=).*(\\,)", "password=*****,"));
            Logger.LogLine("- DestinationDatabase: {0}", config.DestinationDatabase);
            Logger.LogLine("- DestinationKeyPrefix: {0}", config.DestinationKeyPrefix);
            Logger.LogLine("- DestinationKeyReplaceForEmpty: {0}", config.DestinationKeyReplaceForEmpty);
            Logger.LogLine("- DestinationReplace: {0}", config.DestinationReplace);
            Logger.LogLine("");
        }

        public static class Logger
        {
            public static void LogLineWithLevel(string logLevel, string message, params object[] args)
            {
                var finalMessage = $"[{GetCurrentDate()}][{logLevel}] {message ?? ""}";
                Console.WriteLine(finalMessage, args);
            }

            public static void LogWithLevel(string logLevel, string message, params object[] args)
            {
                var finalMessage = $"[{GetCurrentDate()}][{logLevel}] {message ?? ""}";
                Console.Write(finalMessage, args);
            }

            public static void LogLine(string message, params object[] args)
            {
                Console.WriteLine(message, args);
            }

            public static void Log(string message, params object[] args)
            {
                Console.Write(message, args);
            }

            private static string GetCurrentDate()
            {
                return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
