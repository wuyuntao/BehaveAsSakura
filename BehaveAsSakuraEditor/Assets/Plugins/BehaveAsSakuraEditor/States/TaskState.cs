using BehaveAsSakura.Tasks;
using System.Collections.Generic;

namespace BehaveAsSakura.Editor
{
    public class TaskState : EditorState
    {
        public static string GetId(uint taskId)
        {
            return string.Format("Task-{0}", taskId);
        }

        public TaskDescWrapper Desc { get; set; }

        public List<uint> ChildTaskIds { get; set; }

        public TaskState(string id)
            : base(id)
        {
            ChildTaskIds = new List<uint>();
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
