using BehaveAsSakura.Attributes;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(TaskDescWrapper), 3)]
    class CompositeTaskDescWrapper : TaskDescWrapper
    {
        [BehaveAsField(1)]
        public uint[] ChildTaskIds { get; set; }
    }

    public abstract class CompositeTask : Task
    {
        private Task[] childTasks;

        protected CompositeTask(BehaviorTree tree, Task parentTask, uint id, ITaskDesc description, ITaskProps props = null)
            : base(tree, parentTask, id, description, props)
        { }

        internal void InitializeChildren(Task[] childTasks)
        {
            if (this.childTasks != null)
                throw new InvalidOperationException("Child tasks are already initialized");

            this.childTasks = childTasks;
        }

        protected override void OnAbort()
        {
            foreach (var child in childTasks)
                child.EnqueueForAbort();

            base.OnAbort();
        }

        void ThrowIfChildTasksNotInitialized()
        {
            if (childTasks == null)
                throw new InvalidOperationException("Child tasks are not initialized yet");
        }

        protected Task GetChildTask(int index)
        {
            ThrowIfChildTasksNotInitialized();

            return childTasks[index];
        }

        protected IEnumerable<Task> ChildTasks
        {
            get
            {
                ThrowIfChildTasksNotInitialized();

                return childTasks;
            }
        }

        protected int ChildTaskCount
        {
            get
            {
                ThrowIfChildTasksNotInitialized();

                return childTasks.Length;
            }
        }
    }
}
