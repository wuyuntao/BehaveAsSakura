using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 8)]
    public class ReturnFailureTaskDesc : IDecoratorTaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new ReturnFailureTask(tree, parentTask, id, this);
        }
    }

    class ReturnFailureTask : DecoratorTask
    {
        public ReturnFailureTask(BehaviorTree tree, Task parentTask, uint id, ReturnFailureTaskDesc description)
            : base(tree, parentTask, id, description)
        { }

        protected override void OnStart()
        {
            base.OnStart();

            ChildTask.EnqueueForUpdate();
        }

        protected override TaskResult OnUpdate()
        {
            return ChildTask.LastResult == TaskResult.Running ? TaskResult.Running : TaskResult.Failure;
        }
    }
}
