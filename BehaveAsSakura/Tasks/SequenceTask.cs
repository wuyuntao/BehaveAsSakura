using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class SequenceTaskDesc : ITaskDesc
    {
        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new SequenceTask(tree, parentTask, id, this);
        }
    }

    [ProtoContract]
    class SequenceTaskProps : ITaskProps
    {
        [ProtoMember(1)]
        public int CurrentChildIndex { get; set; }
    }

    class SequenceTask : CompositeTask
    {
        private SequenceTaskProps props;

        public SequenceTask(BehaviorTree tree, Task parentTask, uint id, SequenceTaskDesc description)
            : this(tree, parentTask, id, description, new SequenceTaskProps())
        {
            props = (SequenceTaskProps)Props;
        }

        protected SequenceTask(BehaviorTree tree, Task parentTask, uint id, SequenceTaskDesc description, SequenceTaskProps props)
            : base(tree, parentTask, id, description, props)
        {
            props = (SequenceTaskProps)Props;
        }

        protected override void OnStart()
        {
            base.OnStart();

            props.CurrentChildIndex = 0;
            GetChildTask(props.CurrentChildIndex).EnqueueForUpdate();
        }

        protected override TaskResult OnUpdate()
        {
            return IterateChildTasks(TaskResult.Success);
        }

        protected TaskResult IterateChildTasks(TaskResult expectingResult)
        {
            var child = GetChildTask(props.CurrentChildIndex);
            if (child.LastResult == expectingResult)
            {
                if (++props.CurrentChildIndex < ChildTaskCount)
                {
                    GetChildTask(props.CurrentChildIndex).EnqueueForUpdate();

                    return TaskResult.Running;
                }
            }

            return child.LastResult;
        }
    }
}
