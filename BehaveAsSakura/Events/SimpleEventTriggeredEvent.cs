using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Events
{
    [BehaveAsContract]
    public class SimpleEventTriggeredEvent : IEvent
    {
        [BehaveAsMember(1)]
        public string EventType { get; set; }

        public SimpleEventTriggeredEvent(string eventType)
        {
            EventType = eventType;
        }
    }
}
