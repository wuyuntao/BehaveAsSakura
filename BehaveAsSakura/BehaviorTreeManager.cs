using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura
{
	public interface IBehaviorTreeLoader
	{
		BehaviorTreeDesc LoadTree(string path);
	}

	class TaskFactory
	{
		//public Task Create
	}

	public sealed class BehaviorTreeManager
	{
		private IBehaviorTreeLoader loader;
		private Dictionary<Type, TaskFactory> taskFactories = new Dictionary<Type, TaskFactory>();

		public BehaviorTreeManager(IBehaviorTreeLoader loader)
		{
			this.loader = loader;
		}

		public BehaviorTree CreateTree(IBehaviorTreeOwner owner, string path, Task parentTask = null)
		{
			var treeDesc = loader.LoadTree( path );
			var tree = new BehaviorTree( this, owner, treeDesc, parentTask );

			return tree;
		}

		internal Task CreateTask(BehaviorTree tree, uint taskId, Task parentTask = null)
		{
			throw new NotImplementedException();
		}
	}
}
