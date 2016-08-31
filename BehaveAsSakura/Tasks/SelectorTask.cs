using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class SelectorTaskDesc : SequenceTaskDesc
	{
	}

	class SelectorTask : SequenceTask
	{
		public SelectorTask(BehaviorTree tree, Task parentTask, uint id, uint[] childTaskIds, SelectorTaskDesc description)
			: base( tree, parentTask, id, childTaskIds, description )
		{ }

		protected override TaskResult OnUpdate()
		{
			return IterateChildTasks( TaskResult.Failure );
		}
	}
}