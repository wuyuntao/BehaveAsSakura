using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class ReturnFailureTaskDesc : ITaskDesc
	{
	}

	class ReturnFailureTask : DecoratorTask
	{
		public ReturnFailureTask(BehaviorTree tree, Task parentTask, uint id, uint childTaskId, ReturnFailureTaskDesc description)
			: base( tree, parentTask, id, childTaskId, description )
		{ }

		protected override void OnStart()
		{
			base.OnStart();

			ChildTask.EnqueueForUpdate();
		}

		protected override TaskResult OnUpdate()
		{
			return ChildTask.LastResult == TaskResult.Running ? TaskResult.Running : TaskResult.Failure;
		}
	}
}
