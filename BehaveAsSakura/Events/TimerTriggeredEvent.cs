using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Events
{
    [BehaveAsTable]
    public class TimerTriggeredEvent : IEvent
    {
        [BehaveAsField(1)]
        public uint TimerId { get; set; }

        public TimerTriggeredEvent(uint timerId)
        {
            TimerId = timerId;
        }
    }
}
