namespace BehaveAsSakura.Tasks
{
	public abstract class DecoratorTask : Task
	{
		private Task childTask;

		protected DecoratorTask(BehaviorTree tree, Task parentTask, uint id, uint childTaskId, ITaskDesc description, ITaskProps props = null)
			: base( tree, parentTask, id, description, props )
		{
			childTask = Tree.TreeManager.CreateTask( Tree, childTaskId, this );
		}

		protected override void OnAbort()
		{
			childTask.EnqueueForAbort();

			base.OnAbort();
		}

		protected Task ChildTask
		{
			get { return childTask; }
		}
	}
}
