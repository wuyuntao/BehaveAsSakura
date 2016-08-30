using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura
{
	public sealed class BehaviorTreeBuilder
	{
		private uint maxTaskId;
		private List<TaskDesc> tasks = new List<TaskDesc>();

		public CompositeTaskBuilder Composite<T>(string name, Action<T> initializer = null)
			where T : CompositeTaskDesc, new()
		{
			var description = CreateDescription<T>( name, initializer );

			return new CompositeTaskBuilder( description );
		}

		public CompositeTaskBuilder Composite<T>(Action<T> initializer = null)
			where T : CompositeTaskDesc, new()
		{
			return Composite<T>( null, initializer );
		}

		public DecoratorTaskBuilder Decorator<T>(string name, Action<T> initializer = null)
			where T : DecoratorTaskDesc, new()
		{
			var description = CreateDescription<T>( name, initializer );

			return new DecoratorTaskBuilder( description );
		}

		public DecoratorTaskBuilder Decorator<T>(Action<T> initializer = null)
			where T : DecoratorTaskDesc, new()
		{
			return Decorator<T>( null, initializer );
		}

		public LeafTaskBuilder Leaf<T>(string name, Action<T> initializer = null)
			where T : LeafTaskDesc, new()
		{
			var description = CreateDescription<T>( name, initializer );

			return new LeafTaskBuilder( description );
		}

		public LeafTaskBuilder Leaf<T>(Action<T> initializer = null)
			where T : LeafTaskDesc, new()
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

		T CreateDescription<T>(string name, Action<T> initializer)
			where T : TaskDesc, new()
		{
			var description = new T();
			description.Id = ++maxTaskId;
			description.Name = name != null ? name : string.Format( "{0}-{1}", typeof( T ).Name, description.Id );

			initializer?.Invoke( description );

			tasks.Add( description );

			return description;
		}
	}

	public abstract class TaskBuilder
	{
		private TaskDesc description;

		protected TaskBuilder(TaskDesc description)
		{
			this.description = description;
		}

		internal TaskDesc Description
		{
			get { return description; }
		}
	}

	public sealed class CompositeTaskBuilder : TaskBuilder
	{
		private CompositeTaskDesc description;

		public CompositeTaskBuilder(CompositeTaskDesc description)
			: base( description )
		{
			this.description = description;
		}

		public CompositeTaskBuilder AppendChild(TaskBuilder builder)
		{
			if( description.Children == null )
			{
				description.Children = new uint[] { builder.Description.Id };
			}
			else
			{
				var children = new uint[description.Children.Length + 1];
				Array.Copy( description.Children, children, description.Children.Length );
				children[description.Children.Length] = builder.Description.Id;
				description.Children = children;
			}

			return this;
		}
	}

	public sealed class DecoratorTaskBuilder : TaskBuilder
	{
		private DecoratorTaskDesc description;

		public DecoratorTaskBuilder(DecoratorTaskDesc description)
			: base( description )
		{
			this.description = description;
		}

		public DecoratorTaskBuilder SetChild(TaskBuilder builder)
		{
			description.Child = builder.Description.Id;

			return this;
		}
	}

	public sealed class LeafTaskBuilder : TaskBuilder
	{
		public LeafTaskBuilder(LeafTaskDesc description)
			: base( description )
		{
		}
	}
}
