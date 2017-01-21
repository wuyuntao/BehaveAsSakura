using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 14)]
    public class UntilFailureTaskDesc : RepeaterTaskDesc, IDecoratorTaskDesc
    {
        void ITaskDesc.Validate()
        {
        }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new UntilFailureTask(tree, parentTask, id, this);
        }
    }

    class UntilFailureTask : RepeaterTask
    {
        public UntilFailureTask(BehaviorTree tree, Task parentTask, uint id, UntilFailureTaskDesc description)
            : base(tree, parentTask, id, description)
        {
        }

        protected override bool IsRepeaterCompleted(TaskResult result)
        {
            return result == TaskResult.Failure;
        }
    }
}
