using System;
using NLog;

namespace OpenShopVHBackend.BussinessLogic
{
    public class MyLogger : ILogger
    {
        private Logger nlog = LogManager.GetCurrentClassLogger();
        private static MyLogger _logger;

        public static MyLogger GetInstance
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new MyLogger();
                }
                return _logger;
            }
        }

        private MyLogger() { }

        public void Debug(string message)
        {
            nlog.Debug(message);
        }

        public void Error(string message)
        {
            nlog.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            nlog.Error(exception, message);
        }

        public void Fatal(string message)
        {
            nlog.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            nlog.Fatal(message);
        }

        public void Info(string message)
        {
            nlog.Info(message);
        }

        public void Trace(string message)
        {
            nlog.Trace(message);
        }

        public void Warning(string message)
        {
            nlog.Warn(message);
        }
    }
}