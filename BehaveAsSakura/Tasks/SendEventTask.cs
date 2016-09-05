﻿using BehaveAsSakura.Events;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class SendEventTaskDesc : ITaskDesc
    {
        [ProtoMember(1)]
        public string EventType { get; set; }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new SendEventTask(tree, parentTask, id, this);
        }
    }

    class SendEventTask : LeafTask
    {
        private SendEventTaskDesc description;

        public SendEventTask(BehaviorTree tree, Task parentTask, uint id, SendEventTaskDesc description)
            : base(tree, parentTask, id, description)
        {
            this.description = description;
        }

        protected override TaskResult OnUpdate()
        {
            PublishEvent(new SimpleEventTriggeredEvent(description.EventType));

            return TaskResult.Success;
        }
    }
}
