using System;

namespace BehaveAsSakura.Editor
{
    public class TaskCreatedEvent : EditorEvent
    {
        public TaskState NewTask { get; set; }

        public TaskCreatedEvent(string id)
            : base(id)
        {
        }
    }

    public class TaskNotCreatedEvent : EditorEvent
    {
        public string Reason { get; set; }

        public TaskNotCreatedEvent(string id)
            : base(id)
        {
        }
    }
}
