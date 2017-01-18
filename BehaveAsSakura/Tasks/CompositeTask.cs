using BehaveAsSakura.Attributes;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura.Tasks
{
    public interface ICompositeTaskDesc : ITaskDesc
    {
    }

    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(TaskDescWrapper), 3)]
    public class CompositeTaskDescWrapper : TaskDescWrapper
    {
        [BehaveAsField(1)]
        public List<uint> ChildTaskIds { get; set; }

        public CompositeTaskDescWrapper()
        {
            ChildTaskIds = new List<uint>();
        }
    }

    public abstract class CompositeTask : Task
    {
        private Task[] childTasks;

        protected CompositeTask(BehaviorTree tree, Task parentTask, uint id, ICompositeTaskDesc description, ITaskProps props = null)
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
