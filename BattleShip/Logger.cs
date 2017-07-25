using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip
{
    public static class Logger
    {
        private static List<Event> Events = new List<Event>();
        public static EventPriority LogLevel = EventPriority.Debug;

        private static void WriteEvent(EventPriority priority, string eventDescription) // 3 - error, 2 - ?, 1 - common event, 0 - шлак
        {
            Events.Add(new Event(priority, eventDescription));
        }

        public static void WriteInfo(string eventDescription)
        {
            WriteEvent(EventPriority.Info, eventDescription);
        }

        public static void WriteDebug(string eventDescription)
        {
            WriteEvent(EventPriority.Debug, eventDescription);
        }

        public static void WriteError(string eventDescription)
        {
            WriteEvent(EventPriority.Error, eventDescription);
        }

        public static void Flush()
        {
            foreach (var currentEvent in Events)
            {
                if (currentEvent.Priority >= LogLevel)
                    Console.WriteLine(currentEvent.EventDescription);
            }

            Events.Clear();
        }
    }

    public struct Event
    {
        public EventPriority Priority;
        public string EventDescription;

        public Event(EventPriority priority, string eventDescription)
        {
            Priority = priority;
            EventDescription = eventDescription;
        }
    }

            public enum EventPriority
        {
            Info,
            Debug,
            Error
        }
}
