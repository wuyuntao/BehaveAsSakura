using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Events
{
    [BehaveAsTable]
    public class SimpleEventTriggeredEvent : IEvent
    {
        [BehaveAsField(1)]
        public string EventType { get; set; }

        public SimpleEventTriggeredEvent(string eventType)
        {
            EventType = eventType;
        }
    }
}
