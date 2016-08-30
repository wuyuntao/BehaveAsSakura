using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class InverterTaskDesc : DecoratorTaskDesc
	{
	}

	public sealed class InverterTask : DecoratorTask
	{
		public InverterTask(BehaviorTree tree, Task parent, InverterTaskDesc description)
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
			switch( ChildTask.LastResult )
			{
				case TaskResult.Failure:
					return TaskResult.Success;

				case TaskResult.Success:
					return TaskResult.Failure;

				default:
					return TaskResult.Running;
			}
		}
	}
}
