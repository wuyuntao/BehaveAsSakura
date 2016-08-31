using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class UntilFailureDesc : RepeaterTaskDesc
	{
	}

	class UntilFailureTask : RepeaterTask
	{
		public UntilFailureTask(BehaviorTree tree, Task parentTask, uint id, uint childTaskId, UntilFailureDesc description)
			: base( tree, parentTask, id, childTaskId, description )
		{
		}

		protected override bool IsRepeaterCompleted(TaskResult result)
		{
			return result == TaskResult.Failure;
		}
	}
}
