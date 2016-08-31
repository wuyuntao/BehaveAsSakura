using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura
{
    public interface IBehaviorTreeLoader
    {
        BehaviorTreeDesc LoadTree(string path);
    }

    public sealed class BehaviorTreeManager
    {
        private IBehaviorTreeLoader loader;

        public BehaviorTreeManager(IBehaviorTreeLoader loader)
        {
            this.loader = loader;
        }

        public BehaviorTree CreateTree(IBehaviorTreeOwner owner, string path, Task parentTask = null)
        {
            var treeDesc = loader.LoadTree(path);
            var tree = new BehaviorTree(this, owner, treeDesc, parentTask);

            return tree;
        }

        internal Task CreateTask(BehaviorTree tree, BehaviorTreeDesc treeDesc, Task parentTask, uint taskId)
        {
            var descWrapper = treeDesc.FindTaskDesc(taskId);
            var desc = (ITaskDesc)descWrapper.CustomDesc;
            var task = desc.CreateTask(tree, parentTask, descWrapper.Id);

            if (task is LeafTask)
                return task;

            var decoratorTask = task as DecoratorTask;
            if (decoratorTask != null)
            {
                var childTaskId = ((DecoratorTaskDescWrapper)descWrapper).ChildTask;
                var childTask = CreateTask(tree, treeDesc, task, childTaskId);
                decoratorTask.InitializeChild(childTask);
                return decoratorTask;
            }

            var compositeTask = task as CompositeTask;
            if (compositeTask != null)
            {
                var childTaskIds = ((CompositeTaskDescWrapper)descWrapper).ChildTasks;
                var childTasks = Array.ConvertAll(childTaskIds, i => CreateTask(tree, treeDesc, task, i));
                compositeTask.InitializeChildren(childTasks);
                return compositeTask;
            }

            return task;
        }
    }
}
