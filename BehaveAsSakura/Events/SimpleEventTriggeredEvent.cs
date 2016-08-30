using ProtoBuf;

namespace BehaveAsSakura.Events
{
    [ProtoContract]
    public class SimpleEventTriggeredEvent : IEvent
    {
        [ProtoMember(1)]
        public string EventType { get; set; }

        public SimpleEventTriggeredEvent(string eventType)
        {
            EventType = eventType;
        }
    }
}
