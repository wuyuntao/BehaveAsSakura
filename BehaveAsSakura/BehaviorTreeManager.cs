using BehaveAsSakura.Tasks;
using System;

namespace BehaveAsSakura
{
	public class BehaviorTreeManager
	{
		public BehaviorTreeDesc LoadTree(string path)
		{
			throw new NotImplementedException();
		}

		public BehaviorTree CreateTree(IBehaviorTreeOwner owner, string path, Task parentTask = null)
		{
			var treeDesc = LoadTree( path );
			var tree = new BehaviorTree( this, owner, treeDesc, parentTask );

			return tree;
		}

		internal Task CreateTask(BehaviorTree tree, uint taskId, Task parentTask = null)
		{
			throw new NotImplementedException();
		}
	}
}
