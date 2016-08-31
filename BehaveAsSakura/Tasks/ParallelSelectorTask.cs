using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class ParallelSelectorTaskDesc : ParallelTaskDesc
	{
	}

	class ParallelSelectorTask : ParallelTask
	{
		public ParallelSelectorTask(BehaviorTree tree, Task parentTask, uint id, uint[] childTaskIds, ParallelTaskDesc description)
			: base( tree, parentTask, id, childTaskIds, description )
		{ }

		protected override TaskResult OnUpdate()
		{
			return IterateChildTasks( TaskResult.Failure );
		}
	}
}