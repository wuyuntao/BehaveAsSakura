using BehaveAsSakura.Events;
using BehaveAsSakura.Timers;
using BehaveAsSakura.Variables;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class WaitTimerTaskDesc : TaskDesc
	{
		[ProtoMember( 1 )]
		public VariableDesc Time { get; set; }
	}

	[ProtoContract]
	class WaitTimerTaskProps : TaskProps
	{
		[ProtoMember( 1 )]
		public TimerProps Timer;

		[ProtoMember( 2 )]
		public bool IsTimerTriggered;

		public WaitTimerTaskProps(uint id)
			: base( id )
		{ }
	}

	public sealed class WaitTimerTask : LeafTask
	{
		private WaitTimerTaskDesc description;
		private WaitTimerTaskProps props;
		private Variable timeVariable;
		private Timer timer;

		public WaitTimerTask(BehaviorTree tree, Task parent, WaitTimerTaskDesc description)
			: base( tree, parent, description, new WaitTimerTaskProps( description.Id ) )
		{
			this.description = description;
			props = (WaitTimerTaskProps)Props;
			timeVariable = new Variable( description.Time );
		}

		protected override void OnStart()
		{
			base.OnStart();

			props.IsTimerTriggered = false;
			timer = Tree.TimerManager.StartTimer( timeVariable.GetUInt( this ) );

			Owner.EventBus.Subscribe<TimerTriggeredEvent>( this );
		}

		protected override TaskResult OnUpdate()
		{
			return props.IsTimerTriggered ? TaskResult.Success : TaskResult.Running;
		}

		protected override void OnEnd()
		{
			if( !props.IsTimerTriggered && props.Timer != null )
				Tree.TimerManager.CancelTimer( timer );

			Owner.EventBus.Unsubscribe<TimerTriggeredEvent>( this );

			base.OnEnd();
		}

		protected override void OnEventTriggered(IEvent @event)
		{
			base.OnEventTriggered( @event );

			if( !props.IsTimerTriggered )
			{
				var e = @event as TimerTriggeredEvent;
				if( e != null && e.TimerId == timer.Id )
				{
					props.IsTimerTriggered = true;

					Owner.EventBus.Unsubscribe<TimerTriggeredEvent>( this );

					EnqueueForUpdate();
				}
			}
		}
	}
}
