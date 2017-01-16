using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class ParallelSelectorTaskDesc : ParallelTaskDesc, ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new ParallelSelectorTask(tree, parentTask, id, this);
        }
    }

    class ParallelSelectorTask : ParallelTask
    {
        public ParallelSelectorTask(BehaviorTree tree, Task parentTask, uint id, ParallelTaskDesc description)
            : base(tree, parentTask, id, description)
        { }

        protected override TaskResult OnUpdate()
        {
            return IterateChildTasks(TaskResult.Failure);
        }
    }
}