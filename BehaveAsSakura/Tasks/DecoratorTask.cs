using System;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    class DecoratorTaskDescWrapper : TaskDescWrapper
    {
        [ProtoMember(1)]
        public uint ChildTask { get; set; }
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
