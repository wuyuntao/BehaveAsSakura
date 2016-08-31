using BehaveAsSakura.Events;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class SendEventTaskDesc : ITaskDesc
	{
		[ProtoMember( 1 )]
		public string EventType { get; set; }
	}

	class SendEventTask : LeafTask
	{
		private SendEventTaskDesc description;

		public SendEventTask(BehaviorTree tree, Task parentTask, uint id, SendEventTaskDesc description)
			: base( tree, parentTask, id, description )
		{
			this.description = description;
		}

		protected override TaskResult OnUpdate()
		{
			Owner.EventBus.Publish( new SimpleEventTriggeredEvent( description.EventType ) );

			return TaskResult.Success;
		}
	}
}
