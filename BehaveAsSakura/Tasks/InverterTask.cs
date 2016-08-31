using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class InverterTaskDesc : ITaskDesc
	{
	}

	class InverterTask : DecoratorTask
	{
		public InverterTask(BehaviorTree tree, Task parentTask, uint id, uint childTaskId, InverterTaskDesc description)
			: base( tree, parentTask, id, childTaskId, description )
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
