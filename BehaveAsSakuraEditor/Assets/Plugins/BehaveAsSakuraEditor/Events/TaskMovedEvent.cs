using System;

namespace BehaveAsSakura.Editor
{
    public class TaskMovedEvent : EditorEvent
    {
        public bool Left { get; set; }

        public TaskMovedEvent(string id)
            : base(id)
        {
        }
    }

    public class TaskNotMovedEvent : EditorEvent
    {
        public string Reason { get; set; }

        public TaskNotMovedEvent(string id)
            : base(id)
        {
        }
    }
}
