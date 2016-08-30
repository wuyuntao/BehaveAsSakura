using ProtoBuf;

namespace BehaveAsSakura.Events
{
	[ProtoContract]
	public class TimerTriggeredEvent : IEvent
	{
		[ProtoMember( 1 )]
		public uint TimerId { get; set; }

		public TimerTriggeredEvent(uint timerId)
		{
			TimerId = timerId;
		}
	}
}
