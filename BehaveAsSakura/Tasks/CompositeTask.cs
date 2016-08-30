using ProtoBuf;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class CompositeTaskDesc : TaskDesc
	{
		[ProtoMember( 1 )]
		public uint[] Children;
	}

	[ProtoContract]
	public class CompositeTaskProps : TaskProps
	{
		[ProtoMember( 1 )]
		public TaskProps[] Children;

		public CompositeTaskProps(uint id)
			: base( id )
		{ }
	}

	public abstract class CompositeTask : Task
	{
		Task[] childTasks;

		protected CompositeTask(BehaviorTree tree, Task parent, CompositeTaskDesc description, CompositeTaskProps props)
			: base( tree, parent, description, props )
		{
			childTasks = Array.ConvertAll( description.Children, id => Tree.TreeManager.CreateTask( Tree, id, this ) );
		}

		protected override void OnAbort()
		{
			foreach( var child in childTasks )
				child.EnqueueForAbort();

			base.OnAbort();
		}

		protected Task GetChildTask(int index)
		{
			return childTasks[index];
		}

		protected IEnumerable<Task> ChildTasks
		{
			get { return childTasks; }
		}

		protected int ChildTaskCount
		{
			get { return childTasks.Length; }
		}
	}
}
