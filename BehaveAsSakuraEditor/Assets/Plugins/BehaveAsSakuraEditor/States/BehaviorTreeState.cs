using System;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeState : EditorState
    {
        public static string GetId()
        {
            return "BehaviorTree";
        }

        public string FilePath { get; private set; }

        public uint RootTaskId { get; private set; }

        public uint NextTaskId { get; set; }

        public BehaviorTreeState(EditorDomain domain, string id)
            : base(domain, id)
        {
            NextTaskId = 1;
        }

        public override void ApplyEvent(EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                OnTaskCreatedEvent((TaskCreatedEvent)e);
            }
            else if (e is TaskRemovedEvent)
            {
            }

            base.ApplyEvent(e);
        }

        private void OnTaskCreatedEvent(TaskCreatedEvent e)
        {
            Repository.States[e.NewTask.Id] = e.NewTask;

            RootTaskId = e.NewTask.Desc.Id;
            NextTaskId = Math.Max(NextTaskId, e.NewTask.Desc.Id) + 1;

            NodeLayoutHelper.Calculate(this);
        }
    }
}
