using System;

namespace BehaveAsSakura.Editor
{
    class TaskCreatedEvent : EditorEvent
    {
        public Type TaskType { get; set; }

        public TaskCreatedEvent(string id)
            : base(id)
        {
        }
    }

    class TaskNotCreatedEvent : EditorEvent
    {
        public string Reason { get; set; }

        public TaskNotCreatedEvent(string id)
            : base(id)
        {
        }
    }
}
