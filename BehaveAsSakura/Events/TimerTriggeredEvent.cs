using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Events
{
    [BehaveAsContract]
    public class TimerTriggeredEvent : IEvent
    {
        [BehaveAsMember(1)]
        public uint TimerId { get; set; }

        public TimerTriggeredEvent(uint timerId)
        {
            TimerId = timerId;
        }
    }
}
