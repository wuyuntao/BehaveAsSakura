﻿using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 5)]
    [Task("Flow Control/Parallel Selector")]
    public class ParallelSelectorTaskDesc : ParallelTaskDesc, ICompositeTaskDesc
    {
        void ITaskDesc.Validate()
        {
        }

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