using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventsSystem
{
    public class Event
    {
        private static readonly EventHandler.EventPriority[] Order =
        {
            EventHandler.EventPriority.High,
            EventHandler.EventPriority.Normal,
            EventHandler.EventPriority.Low
        };

        public static void RegisterListener(IEventListener listener)
        {
            foreach (var info in listener.GetType().GetMethods())
            {
                var attributes = info.GetCustomAttributes(typeof(EventHandler));
                var pInfo = info.GetParameters();

                if (pInfo.Length == 1)
                {
                    var fInfo = pInfo[0].ParameterType.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
                    EventHandlerContainer hc = null;
                    foreach (var propertyInfo in fInfo)
                        if (propertyInfo.GetValue(null).GetType() == typeof(EventHandlerContainer))
                        {
                            hc = (EventHandlerContainer) propertyInfo.GetValue(null);
                            break;
                        }

                    if (hc == null)
                    {
                        if (pInfo[0].GetType() != typeof(ParameterInfo))
                            Call(new EventLogging(
                                "Trying to Register Listener to Event '" + pInfo[0].GetType() +
                                "' and it has no HandlerContainer!", EventLogging.LoggingLevel.Warning));
                        continue;
                    }

                    var priority = ((EventHandler) attributes.First()).GetEventPriority();

                    if (!hc.Handlers.ContainsKey(priority))
                        hc.Handlers.Add(priority, new Dictionary<IEventListener, MethodInfo>());

                    try
                    {
                        hc.Handlers[priority].Add(listener, info);
                    }
                    catch (Exception)
                    {
                        Call(new EventLogging(
                            "More than one method handling the same event in the same class [Class: " +
                            listener.GetType().Name + ", Method: " + info.Name + "]", EventLogging.LoggingLevel.Severe));
                    }
                }
            }
        }

        public static void RemoveListener(IEventListener listener)
        {
            foreach (var info in listener.GetType().GetMethods())
            {
                var attributes = info.GetCustomAttributes(typeof(EventHandler));
                var pInfo = info.GetParameters();

                if (pInfo.Length == 1)
                {
                    var fInfo = pInfo[0].ParameterType.GetFields(BindingFlags.NonPublic | BindingFlags.Static);

                    EventHandlerContainer hc = null;
                    foreach (var propertyInfo in fInfo)
                        if (propertyInfo.GetValue(null).GetType() == typeof(EventHandlerContainer))
                        {
                            hc = (EventHandlerContainer) propertyInfo.GetValue(null);
                            break;
                        }

                    if (hc == null)
                    {
                        Call(new EventLogging(
                            "Trying to Unregister Listener to Event '" + pInfo[0].GetType() +
                            "' and it can't access HandlerContainer!", EventLogging.LoggingLevel.Warning));
                        continue;
                    }

                    var priority = ((EventHandler) attributes.First()).GetEventPriority();

                    hc.Handlers[priority].Remove(listener);
                }
            }
        }

        public static T Call<T>(T data) where T : Event
        {
            EventHandlerContainer hc = null;
            var pInfo = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var info in pInfo)
                if (info.GetValue(null).GetType() == typeof(EventHandlerContainer))
                {
                    hc = (EventHandlerContainer) info.GetValue(null);
                    break;
                }

            if (hc == null)
            {
                Call(new EventLogging("Event '" + data.GetType() + "' was aborted: No Handler Container",
                    EventLogging.LoggingLevel.Warning));
                return data;
            }

            foreach (var priority in Order)
                if (hc.Handlers.ContainsKey(priority))
                    CallEventAction(data, priority, hc);

            return data;
        }

        private static T CallEventAction<T>(T data, EventHandler.EventPriority priority, EventHandlerContainer hc)
        {
            ICancellable cancellable = null;

            if (data.GetType().GetInterfaces().Contains(typeof(ICancellable)))
                cancellable = (ICancellable) data;

            var listners = new List<IEventListener>(hc.Handlers[priority].Keys);
            foreach (var listener in listners)
            {
                if (cancellable != null && cancellable.Cancelled)
                    return data;

                try
                {
                    hc.Handlers[priority][listener].Invoke(listener, new object[] {data});
                }
                catch (TargetInvocationException)
                {
                    Call(new EventLogging("Invoke Exception for: " + data.GetType().Name));
                }
            }

            return data;
        }
    }
}