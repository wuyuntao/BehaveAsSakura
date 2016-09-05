using System.Collections.Generic;

namespace BehaveAsSakura.Tasks
{
	class TaskScheduler
	{
		private BehaviorTree tree;
		private Stack<Queue<Task>> stack;
		private Queue<Task> queue = new Queue<Task>();

		public TaskScheduler(BehaviorTree tree)
		{
			this.tree = tree;
		}

		public void Enqueue(Task task)
		{
			queue.Enqueue( task );
		}

		public void Update()
		{
			// First update
			if( stack == null )
			{
				stack = new Stack<Queue<Task>>();
				tree.RootTask.EnqueueForUpdate();
			}

			// Process enqueued tasks until none left
			while( queue.Count > 0 )
			{
				stack.Push( queue );
				queue = new Queue<Task>();

				while( stack.Count > 0 )
				{
					var topQueue = stack.Peek();

					while( topQueue.Count > 0 )
					{
						topQueue.Dequeue().Update();

						if( queue.Count > 0 )
							break;
					}

					if( topQueue.Count == 0 )
						stack.Pop();

					if( queue.Count > 0 )
						break;
				}
			}

			// Cleanup for next fresh update
			if( tree.RootTask.LastResult != TaskResult.Running )
				stack = null;
		}
	}
}