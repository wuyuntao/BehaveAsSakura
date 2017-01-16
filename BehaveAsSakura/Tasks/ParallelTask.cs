using System;
using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsContract]
    public class ParallelTaskDesc : ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new ParallelTask(tree, parentTask, id, this);
        }
    }

    class ParallelTask : CompositeTask
    {
        public ParallelTask(BehaviorTree tree, Task parentTask, uint id, ParallelTaskDesc description)
            : this(tree, parentTask, id, description, null)
        { }

        protected ParallelTask(BehaviorTree tree, Task parentTask, uint id, ParallelTaskDesc description, ITaskProps props)
            : base(tree, parentTask, id, description, props)
        { }

        protected override void OnStart()
        {
            base.OnStart();

            foreach (var child in ChildTasks)
                child.EnqueueForUpdate();
        }

        protected override TaskResult OnUpdate()
        {
            return IterateChildTasks(TaskResult.Success);
        }

        protected TaskResult IterateChildTasks(TaskResult expectingResult)
        {
            var allCompleted = true;

            foreach (var child in ChildTasks)
            {
                if (child.LastResult == TaskResult.Running)
                {
                    allCompleted = false;
                }
                else if (child.LastResult != expectingResult)
                {
                    foreach (var c in ChildTasks)        // Abort other children
                        c.EnqueueForAbort();

                    return child.LastResult;
                }
            }

            if (!allCompleted)
                return TaskResult.Running;

            return expectingResult;
        }
    }
}