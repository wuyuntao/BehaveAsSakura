using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class DecoratorTaskDesc : TaskDesc
	{
		[ProtoMember( 1 )]
		public uint Child;
	}

	[ProtoContract]
	public class DecoratorTaskProps : TaskProps
	{
		[ProtoMember( 1 )]
		public TaskProps Child;

		public DecoratorTaskProps(uint id)
			: base( id )
		{ }
	}

	public abstract class DecoratorTask : Task
	{
		private Task childTask;

		protected DecoratorTask(BehaviorTree tree, Task parent, DecoratorTaskDesc description, DecoratorTaskProps props)
			: base( tree, parent, description, props )
		{
			childTask = Tree.TreeManager.CreateTask( Tree, description.Child, this );
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
