using BehaveAsSakura.Events;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class WaitEventTaskDesc : TaskDesc
	{
		[ProtoMember( 1 )]
		public string EventType { get; set; }
	}

	[ProtoContract]
	class WaitEventTaskProps : TaskProps
	{
		[ProtoMember( 1 )]
		public bool IsEventTriggered { get; set; }

		public WaitEventTaskProps(uint id)
			: base( id )
		{ }
	}

	public sealed class WaitEventTask : LeafTask
	{
		private WaitEventTaskDesc description;
		private WaitEventTaskProps props;

		public WaitEventTask(BehaviorTree tree, Task parent, WaitEventTaskDesc description)
			: base( tree, parent, description, new WaitEventTaskProps( description.Id ) )
		{
			this.description = description;
			props = (WaitEventTaskProps)Props;
		}

		protected override void OnStart()
		{
			base.OnStart();

			props.IsEventTriggered = false;

			Owner.EventBus.Subscribe<SimpleEventTriggeredEvent>( this );
		}

		protected override TaskResult OnUpdate()
		{
			return props.IsEventTriggered ? TaskResult.Success : TaskResult.Running;
		}

		protected override void OnEnd()
		{
			Owner.EventBus.Unsubscribe<SimpleEventTriggeredEvent>( this );

			base.OnEnd();
		}

		protected override void OnEventTriggered(IEvent @event)
		{
			base.OnEventTriggered( @event );

			if( !props.IsEventTriggered )
			{
				var e = @event as SimpleEventTriggeredEvent;
				if( e != null && e.EventType == description.EventType )
				{
					props.IsEventTriggered = true;

					Owner.EventBus.Unsubscribe<SimpleEventTriggeredEvent>( this );

					EnqueueForUpdate();
				}
			}
		}
	}
}