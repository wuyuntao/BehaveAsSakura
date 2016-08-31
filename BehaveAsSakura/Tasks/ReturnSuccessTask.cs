using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class ReturnSuccessTaskDesc : ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new ReturnSuccessTask(tree, parentTask, id, this);
        }
    }

    class ReturnSuccessTask : DecoratorTask
    {
        public ReturnSuccessTask(BehaviorTree tree, Task parentTask, uint id, ReturnSuccessTaskDesc description)
            : base(tree, parentTask, id, description)
        { }

        protected override void OnStart()
        {
            base.OnStart();

            ChildTask.EnqueueForUpdate();
        }

        protected override TaskResult OnUpdate()
        {
            return ChildTask.LastResult == TaskResult.Running ? TaskResult.Running : TaskResult.Success;
        }
    }
}
