using BehaveAsSakura.Events;
using BehaveAsSakura.Timers;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	public enum TaskResult : byte
	{
		Running,
		Success,
		Failure,
	}

	public enum TaskState : byte
	{
		Suspend,            // (WaitForUpdate, WaitForAbort) -> Suspend -> WaitForStart
		WaitForStart,       // Suspend -> WaitForStart -> (WaitForUpdate, WaitForAbort)
		WaitForUpdate,      // WaitForStart -> WaitForUpdate -> (WaitForEnqueue, WaitForAbort, Suspend)
		WaitForAbort,       // (WaitForStart, WaitForUpdate, WaitForEnqueue) -> WaitForAbort -> Suspend
		WaitForEnqueue,     // WaitForUpdate -> WaitForEnqueue -> (WaitForUpdate, WaitForAbort)
	}

	[ProtoContract]
	public class TaskDesc
	{
		[ProtoMember( 1 )]
		public uint Id;
		[ProtoMember( 2, IsRequired = false )]
		public string Name { get; set; }
		[ProtoMember( 3, IsRequired = false )]
		public string Comment { get; set; }
	}

	[ProtoContract]
	public class TaskProps
	{
		[ProtoMember( 1 )]
		public uint Id;
		[ProtoMember( 2 )]
		public TaskState State = TaskState.Suspend;
		[ProtoMember( 3 )]
		public TaskResult LastResult = TaskResult.Running;

		public TaskProps(uint id)
		{
			Id = id;
		}
	}

	public abstract class Task : ILogger, ISubscriber
	{
		private BehaviorTree tree;
		private Task parent;
		private TaskDesc description;
		private TaskProps props;
		private Timer immediateTimer;

		protected Task(BehaviorTree tree, Task parent, TaskDesc description, TaskProps props)
		{
			this.tree = tree;
			this.parent = parent;
			this.description = description;
			this.props = props;
		}

		#region Life-cycle

		internal void Update()
		{
			switch( props.State )
			{
				case TaskState.WaitForStart:
					props.State = TaskState.WaitForUpdate;
					OnStart();
					Update();
					break;

				case TaskState.WaitForUpdate:
					props.State = TaskState.WaitForEnqueue;
					var result = OnUpdate();

					if( result != TaskResult.Running )
					{
						props.State = TaskState.Suspend;
						props.LastResult = result;
						OnEnd();

						if( parent != null )
							parent.EnqueueForUpdate();
					}
					break;


				case TaskState.WaitForAbort:
					props.State = TaskState.Suspend;
					props.LastResult = TaskResult.Failure;
					OnAbort();
					OnEnd();
					break;

				default:
					break;
			}
		}

		internal protected void EnqueueForUpdate()
		{
			switch( props.State )
			{
				case TaskState.Suspend:
					props.LastResult = TaskResult.Running;
					props.State = TaskState.WaitForStart;
					tree.EnqueueTask( this );
					break;

				case TaskState.WaitForEnqueue:
					props.State = TaskState.WaitForUpdate;
					tree.EnqueueTask( this );
					break;

				default:
					LogDebug( "[{0}] ignored update request due to stage {1}", this, props.State );
					break;
			}
		}

		internal protected void EnqueueForAbort()
		{
			switch( props.State )
			{
				case TaskState.WaitForStart:
				case TaskState.WaitForUpdate:
				case TaskState.WaitForEnqueue:
					props.State = TaskState.WaitForAbort;
					tree.EnqueueTask( this );
					break;

				default:
					LogDebug( "[{0}] ignored abort request due to stage {1}", this, props.State );
					break;
			}
		}

		internal protected void EnqueueForNextUpdate()
		{
			if( immediateTimer != null )
				tree.TimerManager.CancelTimer( immediateTimer );

			immediateTimer = tree.TimerManager.StartTimer( 0 );
			Owner.EventBus.Subscribe<TimerTriggeredEvent>( this );
		}

		protected virtual void OnStart()
		{
		}

		protected abstract TaskResult OnUpdate();

		protected virtual void OnEnd()
		{
			Owner.EventBus.Unsubscribe<TimerTriggeredEvent>( this );
		}

		protected virtual void OnAbort()
		{
		}

		protected virtual void OnEventTriggered(IEvent @event)
		{
		}

		#endregion

		#region ILogger

		public void LogDebug(string msg, params object[] args)
		{
			tree.Owner.LogDebug( msg, args );
		}

		public void LogInfo(string msg, params object[] args)
		{
			tree.Owner.LogInfo( msg, args );
		}

		public void LogWarning(string msg, params object[] args)
		{
			tree.Owner.LogWarning( msg, args );
		}

		public void LogError(string msg, params object[] args)
		{
			tree.Owner.LogError( msg, args );
		}

		#endregion

		#region ISubscriber

		void ISubscriber.OnEventTriggered(IEvent @event)
		{
			var timerTriggered = @event as TimerTriggeredEvent;
			if( timerTriggered != null && immediateTimer != null && immediateTimer.Id == timerTriggered.TimerId )
			{
				immediateTimer = null;
				Owner.EventBus.Unsubscribe<TimerTriggeredEvent>( this );
				EnqueueForUpdate();
			}

			OnEventTriggered( @event );
		}

		#endregion

		#region Properties

		internal protected BehaviorTree Tree
		{
			get { return tree; }
		}

		internal protected IBehaviorTreeOwner Owner
		{
			get { return tree.Owner; }
		}

		internal protected Task ParentTask
		{
			get
			{
				if( parent != null )
					return parent;

				if( tree.ParentTask != null )
					return tree.ParentTask;

				return null;
			}
		}

		internal protected TaskDesc Description
		{
			get { return description; }
		}

		protected TaskProps Props
		{
			get { return props; }
		}

		internal protected TaskResult LastResult
		{
			get { return props.LastResult; }
		}

		#endregion
	}
}
