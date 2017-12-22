using System;

namespace EventsSystem
{
    public class EventLogging : Event, ICancellable
    {
        public enum LoggingLevel
        {
            Info = 1,
            Warning = 2,
            Severe = 3,
            None = int.MaxValue
        }

        private static EventHandlerContainer _hc = new EventHandlerContainer();

        private static LoggingLevel _minLoggingLevel = LoggingLevel.Info;

        private static LogFormatter _formatter = new DefaultLogFormatter();

        public EventLogging(string message, LoggingLevel level = LoggingLevel.Info, object relation = null)
        {
            LogMessage = message;
            LogLevel = level;
            Relation = relation;

            if (level < _minLoggingLevel || level == LoggingLevel.None)
                SetCancelled(true);
        }

        public string LogMessage { get; }

        public LoggingLevel LogLevel { get; }

        public object Relation { get; }

        public bool Cancelled { get; private set; }

        public void SetCancelled(bool cancel)
        {
            Cancelled = cancel;
        }

        public static void SetMinLoggingLevel(LoggingLevel level)
        {
            _minLoggingLevel = level;
        }

        public static void SetLogFormatter(LogFormatter formatter)
        {
            _formatter = formatter;
        }

        public override string ToString()
        {
            return _formatter.Log(LogLevel, LogMessage);
        }

        public abstract class LogFormatter
        {
            public abstract string Log(LoggingLevel level, string message);
        }

        public class DefaultLogFormatter : LogFormatter
        {
            public override string Log(LoggingLevel level, string message)
            {
                return "[" + DateTime.Now.ToString("HH:mm:ss") + " " + level + "]: " + message;
            }
        }
    }
}