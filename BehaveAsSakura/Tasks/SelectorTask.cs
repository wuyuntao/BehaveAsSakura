using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class SelectorTaskDesc : SequenceTaskDesc
	{
	}

	public class SelectorTask : SequenceTask
	{
		public SelectorTask(BehaviorTree tree, Task parent, SelectorTaskDesc description)
			: base( tree, parent, description, new SequenceTaskProps( description.Id ) )
		{ }

		protected override TaskResult OnUpdate()
		{
			return IterateChildTasks( TaskResult.Failure );
		}
	}
}