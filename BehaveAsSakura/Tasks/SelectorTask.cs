using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 10)]
    public class SelectorTaskDesc : SequenceTaskDesc, ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new SelectorTask(tree, parentTask, id, this);
        }
    }

    class SelectorTask : SequenceTask
    {
        public SelectorTask(BehaviorTree tree, Task parentTask, uint id, SelectorTaskDesc description)
            : base(tree, parentTask, id, description)
        { }

        protected override TaskResult OnUpdate()
        {
            return IterateChildTasks(TaskResult.Failure);
        }
    }
}