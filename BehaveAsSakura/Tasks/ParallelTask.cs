using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class ParallelTaskDesc : CompositeTaskDesc
	{
	}

	public class ParallelTask : CompositeTask
	{
		public ParallelTask(BehaviorTree tree, Task parent, ParallelTaskDesc description)
			: this( tree, parent, description, new CompositeTaskProps( description.Id ) )
		{ }

		protected ParallelTask(BehaviorTree tree, Task parent, ParallelTaskDesc description, CompositeTaskProps props)
			: base( tree, parent, description, props )
		{ }

		protected override void OnStart()
		{
			base.OnStart();

			foreach( var child in ChildTasks )
				child.EnqueueForUpdate();
		}

		protected override TaskResult OnUpdate()
		{
			return IterateChildTasks( TaskResult.Success );
		}

		protected TaskResult IterateChildTasks(TaskResult expectingResult)
		{
			var allCompleted = true;

			foreach( var child in ChildTasks )
			{
				if( child.LastResult == TaskResult.Running )
				{
					allCompleted = false;
				}
				else if( child.LastResult != expectingResult )
				{
					foreach( var c in ChildTasks )        // Abort other children
						c.EnqueueForAbort();

					return child.LastResult;
				}
			}

			if( !allCompleted )
				return TaskResult.Running;

			return expectingResult;
		}
	}
}