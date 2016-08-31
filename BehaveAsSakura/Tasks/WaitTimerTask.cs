using BehaveAsSakura.Events;
using BehaveAsSakura.Timers;
using BehaveAsSakura.Variables;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class WaitTimerTaskDesc : ITaskDesc
    {
        [ProtoMember(1)]
        public VariableDesc Time { get; set; }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new WaitTimerTask(tree, parentTask, id, this);
        }
    }

    [ProtoContract]
    class WaitTimerTaskProps : ITaskProps
    {
        [ProtoMember(1)]
        public TimerProps Timer { get; set; }

        [ProtoMember(2)]
        public bool IsTimerTriggered { get; set; }
    }

    class WaitTimerTask : LeafTask
    {
        private WaitTimerTaskDesc description;
        private WaitTimerTaskProps props;
        private Variable timeVariable;
        private Timer timer;

        public WaitTimerTask(BehaviorTree tree, Task parentTask, uint id, WaitTimerTaskDesc description)
            : base(tree, parentTask, id, description, new WaitTimerTaskProps())
        {
            this.description = description;
            props = (WaitTimerTaskProps)Props;
            timeVariable = new Variable(description.Time);
        }

        protected override void OnStart()
        {
            base.OnStart();

            props.IsTimerTriggered = false;
            timer = Tree.TimerManager.StartTimer(timeVariable.GetUInt(this));

            Owner.EventBus.Subscribe<TimerTriggeredEvent>(this);
        }

        protected override TaskResult OnUpdate()
        {
            return props.IsTimerTriggered ? TaskResult.Success : TaskResult.Running;
        }

        protected override void OnEnd()
        {
            if (!props.IsTimerTriggered && props.Timer != null)
                Tree.TimerManager.CancelTimer(timer);

            Owner.EventBus.Unsubscribe<TimerTriggeredEvent>(this);

            base.OnEnd();
        }

        protected override void OnEventTriggered(IPublisher publisher, IEvent @event)
        {
            base.OnEventTriggered(publisher, @event);

            if (!props.IsTimerTriggered)
            {
                var e = @event as TimerTriggeredEvent;
                if (e != null && e.TimerId == timer.Id)
                {
                    props.IsTimerTriggered = true;

                    Owner.EventBus.Unsubscribe<TimerTriggeredEvent>(this);

                    EnqueueForUpdate();
                }
            }
        }
    }
}
