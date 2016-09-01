﻿using BehaveAsSakura.Events;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class WaitEventTaskDesc : ITaskDesc
    {
        [ProtoMember(1)]
        public string EventType { get; set; }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new WaitEventTask(tree, parentTask, id, this);
        }
    }

    [ProtoContract]
    class WaitEventTaskProps : ITaskProps
    {
        [ProtoMember(1)]
        public bool IsEventTriggered { get; set; }
	}

    class WaitEventTask : LeafTask
    {
        private WaitEventTaskDesc description;
        private WaitEventTaskProps props;

        public WaitEventTask(BehaviorTree tree, Task parentTask, uint id, WaitEventTaskDesc description)
            : base(tree, parentTask, id, description, new WaitEventTaskProps())
        {
            this.description = description;
            props = (WaitEventTaskProps)Props;
        }

        protected override void OnStart()
        {
            base.OnStart();

			SubscribeEvent<SimpleEventTriggeredEvent>();

			props.IsEventTriggered = false;
		}

		protected override TaskResult OnUpdate()
        {
            return props.IsEventTriggered ? TaskResult.Success : TaskResult.Running;
        }

        protected override void OnEnd()
        {
			UnsubscribeEvent<SimpleEventTriggeredEvent>();

            base.OnEnd();
        }

		internal protected override void OnEventTriggered(IEvent @event)
		{
            base.OnEventTriggered( @event );

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
			base.OnRestoreProps( props );

			this.props = (WaitEventTaskProps)props;
		}
	}
}