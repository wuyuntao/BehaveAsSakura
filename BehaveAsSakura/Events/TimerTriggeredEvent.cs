using ProtoBuf;

namespace BehaveAsSakura.Events
{
	[ProtoContract]
	public class TimerTriggeredEvent : IEvent
	{
		[ProtoMember( 1 )]
		public uint TimerId;
	}
}
