using BehaveAsSakura.Attributes;
using System;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(TaskDescWrapper), 2)]
    public class DecoratorTaskDescWrapper : TaskDescWrapper
    {
        [BehaveAsField(1)]
        public uint ChildTaskId { get; set; }
    }

    public abstract class DecoratorTask : Task
    {
        private Task childTask;

        protected DecoratorTask(BehaviorTree tree, Task parentTask, uint id, ITaskDesc description, ITaskProps props = null)
            : base(tree, parentTask, id, description, props)
        { }

        internal void InitializeChild(Task childTask)
        {
            if (this.childTask != null)
                throw new InvalidOperationException("Child task is already initialized");

            this.childTask = childTask;
        }

        protected override void OnAbort()
        {
            childTask.EnqueueForAbort();

            base.OnAbort();
        }

        protected Task ChildTask
        {
            get
            {
                if (childTask == null)
                    throw new InvalidOperationException("Child task is not initialized yet");

                return childTask;
            }
        }
    }
}
