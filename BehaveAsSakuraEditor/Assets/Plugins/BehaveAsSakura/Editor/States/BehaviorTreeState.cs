
using System;

namespace BehaveAsSakura.Editor
{
    class BehaviorTreeState : EditorState
    {
        public string FilePath;

        public TaskState RootTask;

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
