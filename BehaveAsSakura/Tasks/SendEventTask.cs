using BehaveAsSakura.Attributes;
using BehaveAsSakura.Events;
using BehaveAsSakura.Utils;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 11)]
    [Task("Event/Send Event")]
    public class SendEventTaskDesc : ILeafTaskDesc
    {
        [BehaveAsField(1)]
        public string EventType { get; set; }

        void ITaskDesc.Validate()
        {
            Validation.NotEmpty(EventType, nameof(EventType));
        }

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
