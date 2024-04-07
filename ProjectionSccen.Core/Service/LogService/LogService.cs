using System;
using System.IO;
using log4net;
using log4net.Config;

namespace ProjectionSccen.Core.Logs{
    
    /// <summary>
    /// 
    /// </summary>
    public class LogService
    {
        /// <summary>
        /// 
        /// </summary>
        private static ILog log;
        /// <summary>
        /// 
        /// </summary>
        static LogService()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseDirectory, "Config/Log4net.Config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
            log = LogManager.GetLogger(typeof(LogService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Debug(object message)
        {
            log.Debug(message);
        }

        public static void DebugFormatted(string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        public static void Info(object message)
        {
            log.Info(message);
        }

        public static void InfoFormatted(string format, params object[] args)
        {
            log.InfoFormat(format, args);
        }

        public static void Warn(object message)
        {
            log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log.Warn(message, exception);
        }

        public void WarnFormatted(string format, params object[] args)
        {
            log.WarnFormat(format, args);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void ErrorFormatted(string format, params object[] args)
        {
            log.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log.Fatal(message, exception);
        }

        public void FatalFormatted(string format, params object[] args)
        {
            log.FatalFormat(format, args);
        }
    }


}