using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class UntilFailureDesc : RepeaterTaskDesc
	{
	}

	public sealed class UntilFailureTask : RepeaterTask
	{
		public UntilFailureTask(BehaviorTree tree, Task parent, UntilFailureDesc description)
			: base( tree, parent, description, new RepeaterTaskProps( description.Id ) )
		{
		}

		protected override bool IsRepeaterCompleted(TaskResult result)
		{
			return result == TaskResult.Failure;
		}
	}
}
