using System;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class InverterTaskDesc : ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new InverterTask(tree, parentTask, id, this);
        }
    }

    class InverterTask : DecoratorTask
    {
        public InverterTask(BehaviorTree tree, Task parentTask, uint id, InverterTaskDesc description)
            : base(tree, parentTask, id, description)
        { }

        protected override void OnStart()
        {
            base.OnStart();

            ChildTask.EnqueueForUpdate();
        }

        protected override TaskResult OnUpdate()
        {
            switch (ChildTask.LastResult)
            {
                case TaskResult.Failure:
                    return TaskResult.Success;

                case TaskResult.Success:
                    return TaskResult.Failure;

                default:
                    return TaskResult.Running;
            }
        }
    }
}
