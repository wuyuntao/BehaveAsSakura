using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsContract]
    public class UntilSuccessDesc : RepeaterTaskDesc, ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new UntilSuccessTask(tree, parentTask, id, this);
        }
    }

    class UntilSuccessTask : RepeaterTask
    {
        public UntilSuccessTask(BehaviorTree tree, Task parentTask, uint id, UntilSuccessDesc description)
            : base(tree, parentTask, id, description)
        {
        }

        protected override bool IsRepeaterCompleted(TaskResult result)
        {
            return result == TaskResult.Success;
        }
    }
}
