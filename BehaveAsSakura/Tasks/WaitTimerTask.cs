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
        [ProtoMember(1, IsRequired = false)]
        public uint? TimerId { get; set; }

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

            timer = StartTimer(timeVariable.GetUInt(this));
            SubscribeEvent<TimerTriggeredEvent>();

			props.TimerId = timer.Id;
            props.IsTimerTriggered = false;
        }

        protected override TaskResult OnUpdate()
        {
            return props.IsTimerTriggered ? TaskResult.Success : TaskResult.Running;
        }

        protected override void OnEnd()
        {
            if (!props.IsTimerTriggered && timer != null)
                CancelTimer(timer);

            UnsubscribeEvent<TimerTriggeredEvent>();

			props.TimerId = null;

			base.OnEnd();
        }

		internal protected override void OnEventTriggered(IEvent @event)
		{
            base.OnEventTriggered( @event );

            if (!props.IsTimerTriggered)
            {
                var e = @event as TimerTriggeredEvent;
                if (e != null && e.TimerId == timer.Id)
                {
					Tree.EventBus.Unsubscribe<TimerTriggeredEvent>( this );

					props.IsTimerTriggered = true;

                    EnqueueForUpdate();
                }
            }
        }

		protected override void OnRestoreProps(ITaskProps props)
		{
			base.OnRestoreProps( props );

			this.props = (WaitTimerTaskProps)props;

			if( this.props.TimerId != null )
				timer = FindTimer( this.props.TimerId.Value );
			else
				timer = null;
		}
	}
}
