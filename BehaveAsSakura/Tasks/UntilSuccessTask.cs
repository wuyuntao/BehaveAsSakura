using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 15)]
    public class UntilSuccessTaskDesc : RepeaterTaskDesc, ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new UntilSuccessTask(tree, parentTask, id, this);
        }
    }

    class UntilSuccessTask : RepeaterTask
    {
        public UntilSuccessTask(BehaviorTree tree, Task parentTask, uint id, UntilSuccessTaskDesc description)
            : base(tree, parentTask, id, description)
        {
        }

        protected override bool IsRepeaterCompleted(TaskResult result)
        {
            return result == TaskResult.Success;
        }
    }
}
