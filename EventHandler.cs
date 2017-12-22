using System;

namespace EventsSystem
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventHandler : Attribute
    {
        public enum EventPriority
        {
            Low = 1,
            Normal = 2,
            High = 3
        }

        private readonly EventPriority _priority;

        public EventHandler() : this(EventPriority.Normal)
        {
        }

        public EventHandler(EventPriority priority)
        {
            _priority = priority;
        }

        public EventPriority GetEventPriority()
        {
            return _priority;
        }
    }
}