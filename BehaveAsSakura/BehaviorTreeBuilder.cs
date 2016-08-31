using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura
{
	public sealed class BehaviorTreeBuilder
	{
		private uint maxTaskId;
		private List<TaskDescWrapper> tasks = new List<TaskDescWrapper>();

		public CompositeTaskBuilder Composite<T>(string name, Action<T> initializer = null)
			where T : ITaskDesc, new()
		{
			var task = CreateTask<CompositeTaskDescWrapper, T>( name, initializer );

			return new CompositeTaskBuilder( task );
		}

		public CompositeTaskBuilder Composite<T>(Action<T> initializer = null)
			where T : ITaskDesc, new()
		{
			return Composite<T>( null, initializer );
		}

		public DecoratorTaskBuilder Decorator<T>(string name, Action<T> initializer = null)
			where T : ITaskDesc, new()
		{
			var task = CreateTask<DecoratorTaskDescWrapper, T>( name, initializer );

			return new DecoratorTaskBuilder( task );
		}

		public DecoratorTaskBuilder Decorator<T>(Action<T> initializer = null)
			where T : ITaskDesc, new()
		{
			return Decorator<T>( null, initializer );
		}

		public LeafTaskBuilder Leaf<T>(string name, Action<T> initializer = null)
			where T : ITaskDesc, new()
		{
			var task = CreateTask<TaskDescWrapper, T>( name, initializer );

			return new LeafTaskBuilder( task );
		}

		public LeafTaskBuilder Leaf<T>(Action<T> initializer = null)
			where T : ITaskDesc, new()
		{
			return Leaf( null, initializer );
		}

		public BehaviorTreeDesc Build()
		{
			return new BehaviorTreeDesc()
			{
				Tasks = tasks.ToArray(),
				RootTaskId = 1,
			};
		}

		TTaskDescWrapper CreateTask<TTaskDescWrapper, TTaskDesc>(string name, Action<TTaskDesc> initializer)
			where TTaskDescWrapper : TaskDescWrapper, new()
			where TTaskDesc : ITaskDesc, new()
		{
			var id = ++maxTaskId;
			var desc = new TTaskDesc();
			var task = new TTaskDescWrapper()
			{
				Id = id,
				Name = name != null ? name : string.Format( "{0}-{1}", typeof( TTaskDesc ).Name, id ),
				CustomDesc = desc,
			};

			initializer?.Invoke( desc );

			tasks.Add( task );

			return task;
		}
	}

	public abstract class TaskBuilder
	{
		private TaskDescWrapper task;

		internal TaskBuilder(TaskDescWrapper task)
		{
			this.task = task;
		}

		internal TaskDescWrapper Task
		{
			get { return task; }
		}
	}

	public sealed class CompositeTaskBuilder : TaskBuilder
	{
		private CompositeTaskDescWrapper task;

		internal CompositeTaskBuilder(CompositeTaskDescWrapper task)
			: base( task )
		{
			this.task = task;
		}

		public CompositeTaskBuilder AppendChild(TaskBuilder builder)
		{
			if( task.ChildTasks == null )
			{
				task.ChildTasks = new uint[] { builder.Task.Id };
			}
			else
			{
				var children = new uint[task.ChildTasks.Length + 1];
				Array.Copy( task.ChildTasks, children, task.ChildTasks.Length );
				children[task.ChildTasks.Length] = builder.Task.Id;
				task.ChildTasks = children;
			}

			return this;
		}
	}

	public sealed class DecoratorTaskBuilder : TaskBuilder
	{
		private DecoratorTaskDescWrapper task;

		internal DecoratorTaskBuilder(DecoratorTaskDescWrapper task)
			: base( task )
		{
			this.task = task;
		}

		public DecoratorTaskBuilder SetChild(TaskBuilder builder)
		{
			task.ChildTask = builder.Task.Id;

			return this;
		}
	}

	public sealed class LeafTaskBuilder : TaskBuilder
	{
		internal LeafTaskBuilder(TaskDescWrapper task)
			: base( task )
		{
		}
	}
}
