using BehaveAsSakura.Tasks;
using System.Collections.Generic;

namespace BehaveAsSakura.Editor
{
    public class TaskState : EditorState
    {
        public TaskDescWrapper Desc { get; set; }

        public TaskState(string id)
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
