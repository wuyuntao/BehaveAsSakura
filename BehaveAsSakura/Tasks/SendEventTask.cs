using BehaveAsSakura.Events;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{

	[ProtoContract]
	public class SendEventTaskDesc : TaskDesc
	{
		[ProtoMember( 1 )]
		public string EventType { get; set; }
	}

	class SendEventTask : LeafTask
	{
		private SendEventTaskDesc description;

		public SendEventTask(BehaviorTree tree, Task parent, SendEventTaskDesc description)
			: base( tree, parent, description, new TaskProps( description.Id ) )
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
