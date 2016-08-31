using ProtoBuf;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	class CompositeTaskDescWrapper : TaskDescWrapper
	{
		[ProtoMember( 1 )]
		public uint[] ChildTasks { get; set; }
	}

	public abstract class CompositeTask : Task
	{
		private Task[] childTasks;

		protected CompositeTask(BehaviorTree tree, Task parentTask, uint id, uint[] childTaskIds, ITaskDesc description, ITaskProps props = null)
			: base( tree, parentTask, id, description, props )
		{
			childTasks = Array.ConvertAll( childTaskIds, t => Tree.TreeManager.CreateTask( Tree, t, this ) );
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
