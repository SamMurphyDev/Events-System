using System.Collections.Generic;
using System.Reflection;

namespace EventsSystem
{
    public class EventHandlerContainer
    {
        public Dictionary<EventHandler.EventPriority, Dictionary<IEventListener, MethodInfo>> Handlers =
            new Dictionary<EventHandler.EventPriority, Dictionary<IEventListener, MethodInfo>>();
    }
}