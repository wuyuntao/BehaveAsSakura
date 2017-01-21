using System.Collections.Generic;

namespace BehaveAsSakura.Editor
{
    public class TaskPropertyChangedEvent : EditorEvent
    {
        public List<ChangeTaskPropertyCommand.Item> Items { get; set; }

        public TaskPropertyChangedEvent(string id)
            : base(id)
        {
            Items = new List<ChangeTaskPropertyCommand.Item>();
        }
    }
}
