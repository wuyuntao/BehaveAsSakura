using BehaveAsSakura.Attributes;
using BehaveAsSakura.Events;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 3)]
    public class ListenEventTaskDesc : ITaskDesc
    {
        [BehaveAsField(1)]
        public string EventType { get; set; }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new ListenEventTask(tree, parentTask, id, this);
        }
    }

    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskProps), 1)]
    class ListenEventTaskProps : ITaskProps
    {
        [BehaveAsField(1)]
        public bool IsEventTriggered { get; set; }
    }

    class ListenEventTask : DecoratorTask
    {
        private ListenEventTaskDesc description;
        private ListenEventTaskProps props;

        public ListenEventTask(BehaviorTree tree, Task parentTask, uint id, ListenEventTaskDesc description)
            : base(tree, parentTask, id, description, new ListenEventTaskProps())
        {
            this.description = description;
            props = (ListenEventTaskProps)Props;
        }

        protected override void OnStart()
        {
            base.OnStart();

            props.IsEventTriggered = false;

            SubscribeEvent<SimpleEventTriggeredEvent>();

            ChildTask.EnqueueForUpdate();
        }

        protected override TaskResult OnUpdate()
        {
            if (props.IsEventTriggered)
            {
                ChildTask.EnqueueForAbort();

                return TaskResult.Success;
            }

            return ChildTask.LastResult;
        }

        protected override void OnEnd()
        {
            if (!props.IsEventTriggered)
                UnsubscribeEvent<SimpleEventTriggeredEvent>();

            base.OnEnd();
        }

        internal protected override void OnEventTriggered(IEvent @event)
        {
            base.OnEventTriggered(@event);

            if (!props.IsEventTriggered)
            {
                var e = @event as SimpleEventTriggeredEvent;
                if (e != null && e.EventType == description.EventType)
                {
                    props.IsEventTriggered = true;

                    UnsubscribeEvent<SimpleEventTriggeredEvent>();

                    EnqueueForUpdate();
                }
            }
        }

        protected override void OnRestoreProps(ITaskProps props)
        {
            base.OnRestoreProps(props);

            this.props = (ListenEventTaskProps)props;
        }
    }
}