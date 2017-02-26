namespace BehaveAsSakura.Editor
{
    public class TaskCollapseChangedEvent : EditorEvent
    {
        public bool IsCollapsed { get; set; }

        public TaskCollapseChangedEvent(string id)
            : base(id)
        {
        }
    }
}
