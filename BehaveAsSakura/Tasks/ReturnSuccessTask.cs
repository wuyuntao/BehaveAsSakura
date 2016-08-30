using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class ReturnSuccessTaskDesc : DecoratorTaskDesc
	{
	}

	class ReturnSuccessTask : DecoratorTask
	{
		public ReturnSuccessTask(BehaviorTree tree, Task parent, ReturnSuccessTaskDesc description)
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
			return ChildTask.LastResult == TaskResult.Running ? TaskResult.Running : TaskResult.Success;
		}
	}
}
