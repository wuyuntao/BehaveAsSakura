using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class UntilSuccessDesc : RepeaterTaskDesc
	{
	}

	public sealed class UntilSuccessTask : RepeaterTask
	{
		public UntilSuccessTask(BehaviorTree tree, Task parent, UntilSuccessDesc description)
			: base( tree, parent, description, new RepeaterTaskProps( description.Id ) )
		{
		}

		protected override bool IsRepeaterCompleted(TaskResult result)
		{
			return result == TaskResult.Success;
		}
	}
}
