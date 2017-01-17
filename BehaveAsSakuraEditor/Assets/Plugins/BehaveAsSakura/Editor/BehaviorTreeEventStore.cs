using System;

namespace BehaveAsSakura.Editor
{
    class BehaviorTreeEventStore : EditorEventStore<BehaviorTreeDesc>
    {
        public BehaviorTreeEventStore(BehaviorTreeDesc snapshot)
            : base(snapshot)
        {
        }

        public override void ApplyEvent(EditorEvent e)
        {
            if (e is TaskNodeCreatedEvent)
            {
                OnTaskNodeCreatedEvent((TaskNodeCreatedEvent)e);
            }
            else if (e is TaskNodeRemovedEvent)
            {
                OnTaskNodeRemovedEvent((TaskNodeCreatedEvent)e);
            }
            else
                throw new NotSupportedException(e.ToString());

            base.ApplyEvent(e);
        }

        private void OnTaskNodeCreatedEvent(TaskNodeCreatedEvent e)
        {
            throw new NotImplementedException();
        }

        private void OnTaskNodeRemovedEvent(TaskNodeCreatedEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
