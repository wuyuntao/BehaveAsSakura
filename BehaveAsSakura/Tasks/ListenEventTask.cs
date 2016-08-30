using BehaveAsSakura.Events;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class ListenEventTaskDesc : DecoratorTaskDesc
	{
		[ProtoMember( 1 )]
		public string EventType { get; set; }
	}

	[ProtoContract]
	class ListenEventTaskProps : DecoratorTaskProps
	{
		[ProtoMember( 1 )]
		public bool IsEventTriggered { get; set; }

		public ListenEventTaskProps(uint id)
			: base( id )
		{ }
	}

	class ListenEventTask : DecoratorTask
	{
		private ListenEventTaskDesc description;
		private ListenEventTaskProps props;

		public ListenEventTask(BehaviorTree tree, Task parent, ListenEventTaskDesc description)
			: base( tree, parent, description, new ListenEventTaskProps( description.Id ) )
		{
			this.description = description;
			props = (ListenEventTaskProps)Props;
		}

		protected override void OnStart()
		{
			base.OnStart();

			props.IsEventTriggered = false;

			Owner.EventBus.Subscribe<SimpleEventTriggeredEvent>( this );

			ChildTask.EnqueueForUpdate();
		}

		protected override TaskResult OnUpdate()
		{
			if( props.IsEventTriggered )
			{
				ChildTask.EnqueueForAbort();

				return TaskResult.Success;
			}

			return ChildTask.LastResult;
		}

		protected override void OnEnd()
		{
			if( !props.IsEventTriggered )
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