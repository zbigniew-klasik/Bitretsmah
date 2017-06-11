using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Bitretsmah.UI.ConsoleApp
{
    public class Logger : Bitretsmah.Core.Interfaces.ILogger
    {
        private readonly NLog.Logger _nLogger;

        static Logger()
        {
            var config = new LoggingConfiguration();

            var target = new FileTarget
            {
                FileName = @"${basedir}/logs/" + DateTime.UtcNow.Ticks + ".log"
            };

            target.Layout = "${date} | ${level} | ${message} ${exception:format=toString,Data:maxInnerExceptionLevel=10}";

            config.AddTarget("target", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, target));
            LogManager.Configuration = config;
        }

        public Logger()
        {
            _nLogger = LogManager.GetLogger("Bitretsmah");
        }

        public void Info(string message)
        {
            _nLogger.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            _nLogger.Info(message, args);
        }

        public void Warn(Exception exception)
        {
            _nLogger.Warn(exception);
        }

        public void Warn(string message)
        {
            _nLogger.Warn(message);
        }

        public void Warn(Exception exception, string message)
        {
            _nLogger.Warn(exception, message);
        }

        public void Warn(string message, params object[] args)
        {
            _nLogger.Warn(message, args);
        }

        public void Warn(Exception exception, string message, params object[] args)
        {
            _nLogger.Warn(exception, message, args);
        }

        public void Error(Exception exception)
        {
            _nLogger.Error(exception);
        }

        public void Error(string message)
        {
            _nLogger.Error(message);
        }

        public void Error(Exception exception, string message)
        {
            _nLogger.Error(exception, message);
        }

        public void Error(string message, params object[] args)
        {
            _nLogger.Error(message, args);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            _nLogger.Error(exception, message, args);
        }
    }
}