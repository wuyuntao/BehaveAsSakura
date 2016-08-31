using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class UntilSuccessDesc : RepeaterTaskDesc
	{
	}

	class UntilSuccessTask : RepeaterTask
	{
		public UntilSuccessTask(BehaviorTree tree, Task parentTask, uint id, uint childTaskId, UntilSuccessDesc description)
			: base( tree, parentTask, id, childTaskId, description )
		{
		}

		protected override bool IsRepeaterCompleted(TaskResult result)
		{
			return result == TaskResult.Success;
		}
	}
}
