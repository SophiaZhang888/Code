using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveAutoTestService.Logs
{
    public static class Logger
    {
        #region Private variables
        private static LoggingLevelSwitch _levelSwitchFile = new LoggingLevelSwitch();
        #endregion Private variables
        #region Public Methods
        /// <summary>
        /// Sets file sink with default log level to the server.
        /// </summary>
        /// <param name="serverName">The name of the server.</param>
        public static void InitLog(string serverName)
        {
            string baseDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
            addRollingFileSink($@"{baseDirectory}\Logs\", $"{serverName}Log_.txt");
            _levelSwitchFile.MinimumLevel = LogEventLevel.Debug;
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Debug level.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValue">Object positionally formatted into the message template.</param>
        public static void Debug<T>(string messageTemplate, T propertyValue)
        {
            Serilog.Log.Logger.Debug(messageTemplate, propertyValue);
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Debug level.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        public static void Debug(string messageTemplate)
        {
            Serilog.Log.Logger.Debug(messageTemplate);
        }

        public static void Error(Exception exception, string messageTemplate)
        {
            Serilog.Log.Logger.Error(exception, messageTemplate);
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Error level.
        /// </summary>
        /// <remarks>This command will set the results the currently running test case to <c>Failed</c> but won't terminate the execution of the test case.</remarks>
        /// <param name="messageTemplate">Message template describing the event.</param>
        public static void Error(string messageTemplate)
        {
            Serilog.Log.Logger.Error(messageTemplate);
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Error level.
        /// </summary>
        /// <remarks>This command will set the results the currently running test case to <c>Failed</c> but won't terminate the execution of the test case.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValue">Object positionally formatted into the message template.</param>
        public static void Error<T>(string messageTemplate, T propertyValue)
        {
            Serilog.Log.Logger.Error(messageTemplate, propertyValue);
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Information level and associated exception.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
        public static void Info(string messageTemplate, params object[] propertyValues)
        {
            Serilog.Log.Logger.Information(messageTemplate, propertyValues);
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Information level.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValue">Object positionally formatted into the message template.</param>
        public static void Info<T>(string messageTemplate, T propertyValue)
        {
            Serilog.Log.Logger.Information(messageTemplate, propertyValue);
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Information level.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        public static void Info(string messageTemplate)
        {
            Serilog.Log.Logger.Information(messageTemplate);
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Warning level.
        /// </summary>
        /// <param name="messageTemplate">Message template describing the event.</param>
        public static void Warning(string messageTemplate)
        {
            Serilog.Log.Logger.Warning(messageTemplate);
        }
        /// <summary>
        /// Write a log event with the Serilog.Events.LogEventLevel.Warning level.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageTemplate">Message template describing the event.</param>
        /// <param name="propertyValue">Object positionally formatted into the message template.</param>
        public static void Warning<T>(string messageTemplate, T propertyValue)
        {
            Serilog.Log.Logger.Warning(messageTemplate, propertyValue);
        }
        #endregion Public Methods

        #region Private Methods
        private static void addRollingFileSink(string filePath = null, string fileName = "TestLog.txt")
        {
            filePath = filePath ?? $"{AppContext.BaseDirectory}";

            Serilog.ILogger logger = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .WriteTo.Logger(Serilog.Log.Logger)
                        .WriteTo.File($"{filePath}{fileName}",
                        rollingInterval: RollingInterval.Day,
                        levelSwitch: _levelSwitchFile,
                        retainedFileCountLimit: 7)
                        .CreateLogger();

            Serilog.Log.Logger = logger;
        }
        #endregion Private Methods
    }
}
