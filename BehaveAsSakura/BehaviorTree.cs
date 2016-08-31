using BehaveAsSakura.Events;
using BehaveAsSakura.Tasks;
using BehaveAsSakura.Timers;
using ProtoBuf;

namespace BehaveAsSakura
{
	public interface ILogger
	{
		void LogDebug(string msg, params object[] args);

		void LogInfo(string msg, params object[] args);

		void LogWarning(string msg, params object[] args);

		void LogError(string msg, params object[] args);
	}

	public interface IBehaviorTreeOwner : ILogger
	{
		uint CurrentTime { get; }

		EventBus EventBus { get; }
	}

	[ProtoContract]
	public class BehaviorTreeDesc
	{
		[ProtoMember( 1 )]
		internal TaskDescWrapper[] Tasks { get; set; }

		[ProtoMember( 2 )]
		internal uint RootTaskId { get; set; }

		[ProtoAfterDeserialization]
		void Initialize()
		{
		}
	}

	[ProtoContract]
	class BehaviorTreeProps
	{
		[ProtoMember( 1 )]
		public TaskPropsWrapper[] Tasks { get; set; }
	}

	public sealed class BehaviorTree
	{
		private BehaviorTreeManager treeManager;
		private IBehaviorTreeOwner owner;
		private Task parentTask;
		private Task rootTask;
		private TimerManager timerManager;
		private TaskTickQueue taskTickQueue;

		internal BehaviorTree(BehaviorTreeManager treeManager, IBehaviorTreeOwner owner, BehaviorTreeDesc description, Task parentTask)
		{
			this.treeManager = treeManager;
			this.owner = owner;
			this.parentTask = parentTask;
			timerManager = new TimerManager( this );
			taskTickQueue = new TaskTickQueue( this );
			rootTask = treeManager.CreateTask( this, description.RootTaskId, parentTask );
		}

		#region Life-cycle

		public void Update()
		{
			timerManager.Update();

			taskTickQueue.Update();
		}

		public void Abort()
		{
			rootTask.EnqueueForAbort();

			taskTickQueue.Update();
		}

		#endregion

		#region Task Manipulation

		internal void EnqueueTask(Task task)
		{
			taskTickQueue.Enqueue( task );
		}

		#endregion

		internal IBehaviorTreeOwner Owner
		{
			get { return owner; }
		}

		internal Task ParentTask
		{
			get { return parentTask; }
		}

		public Task RootTask
		{
			get { return rootTask; }
		}

		internal BehaviorTreeManager TreeManager
		{
			get { return treeManager; }
		}

		internal TimerManager TimerManager
		{
			get { return timerManager; }
		}
	}
}
