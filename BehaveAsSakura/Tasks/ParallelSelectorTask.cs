using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class ParallelSelectorTaskDesc : ParallelTaskDesc
	{
	}

	public class ParallelSelectorTask : ParallelTask
	{
		public ParallelSelectorTask(BehaviorTree tree, Task parent, ParallelTaskDesc description)
			: base( tree, parent, description, new CompositeTaskProps( description.Id ) )
		{ }

		protected override TaskResult OnUpdate()
		{
			return IterateChildTasks( TaskResult.Failure );
		}
	}
}