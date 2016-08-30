using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class ReturnFailureTaskDesc : DecoratorTaskDesc
	{
	}

	class ReturnFailureTask : DecoratorTask
	{
		public ReturnFailureTask(BehaviorTree tree, Task parent, ReturnFailureTaskDesc description)
			: base( tree, parent, description, new DecoratorTaskProps( description.Id ) )
		{
		}

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
