
using System;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeState : EditorState
    {
        public const string DefaultId = "BehaviorTree";

        public string FilePath { get; private set; }

        public uint RootTaskId { get; private set; }

        public BehaviorTreeState(string id)
            : base(id)
        {
        }

        public override void ApplyEvent(EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
            }
            else if (e is TaskRemovedEvent)
            {
            }

            base.ApplyEvent(e);
        }
    }
}
