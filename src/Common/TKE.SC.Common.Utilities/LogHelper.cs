using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TKE.SC.Common
{
    public class LogHelper
    {
        protected static ILogger _logger;

        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Method used to log the Begin of the method
        /// </summary>
        /// <param Name="caller"></param>
        /// <returns></returns>
        public static DateTime LogBegin(string message = "", [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var startTime = DateTime.Now;

            LogDebug(string.Format(Constants.LOGBEGINSTRINGFORMAT, memberName), GetDefaultSerilogProperties(sourceFilePath, sourceLineNumber));
            LogDebug(message);
            return startTime;

        }

        /// <summary>
        /// Method used to log the End of the method: TO be called before the return or at the end of the method
        /// </summary>
        /// <param Name="caller"></param>
        /// <param Name="startTime"></param>
        public static void LogEnd(DateTime? startTime = null, string message = "", [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var timeSpent = DateTime.Now.Subtract(startTime != null ? (DateTime)startTime : DateTime.Now).TotalMilliseconds;

            LogDebug(message);
            LogDebug(string.Format(Constants.LOGENDSTRINGFORMAT, memberName, timeSpent), GetDefaultSerilogProperties(sourceFilePath, sourceLineNumber));
        }

        /// <summary>
        /// Dictionary Constructor for the basic properties
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        /// <returns></returns>
        private static IDictionary<string, object> GetDefaultSerilogProperties(string sourceFilePath, int sourceLineNumber)
        {
            return new Dictionary<string, object>() { { "File", sourceFilePath }, { "LineNo", sourceLineNumber.ToString() } };
        }

        /// <summary>
        /// Method to log the Trace level info
        /// </summary>
        /// <param name="message"></param>
        public static void LogTrace(string message, IDictionary<string, object> properties = null)
        {
            Log(message, LogLevel.Trace, properties);
        }

        /// <summary>
        /// Method log the Debug
        /// </summary>
        /// <param name="debugMessage"></param>
        /// <param name="properties"></param>
        public static void LogDebug(string debugMessage, IDictionary<string, object> properties = null)
        {
            if (string.IsNullOrEmpty(debugMessage))
            {
                return;
            }
            Log(debugMessage, LogLevel.Debug, properties);
        }

        /// <summary>
        /// Method to log the Information
        /// </summary>
        /// <param name="info"></param>
        public static void LogInfo(string info, IDictionary<string, object> properties = null)
        {
            Log(info, LogLevel.Information, properties);
        }

        /// <summary>
        /// Methid to log the warnings
        /// </summary>
        /// <param name="info"></param>
        /// <param name="properties"></param>
        public static void LogWarning(string info, IDictionary<string, object> properties = null)
        {
            Log(info, LogLevel.Warning, properties);
        }

        /// <summary>
        /// Method to log the Error Info
        /// </summary>
        /// <param name="error"></param>
        public static void LogError(string errorMessage, IDictionary<string, object> properties = null)
        {
            Log(errorMessage, LogLevel.Error, properties);
        }

        /// <summary>
        /// Log factory method to log with additional properties
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <param name="properties"></param>
        private static void Log(string message, LogLevel logLevel = LogLevel.Debug, IDictionary<string, object> properties = null)
        {
            if (properties is null)
            {
                Log(message, logLevel);
                return;
            }
            if( _logger is null)
            {
                return;
            }
            using (_logger.BeginScope(properties))
            {
                Log(message, logLevel);
            }
        }

        /// <summary>
        /// Log generic method to log into files
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        private static void Log(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.LogTrace($"{message}");
                    break;
                case LogLevel.Information:
                    _logger.LogInformation($"{message}");
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning($"{ message}");
                    break;
                case LogLevel.Error:
                    _logger.LogError($"{message}");
                    break;
                case LogLevel.Debug:
                default:
                    _logger.LogDebug($"{ message}");
                    break;
            }
        }
    }
}
