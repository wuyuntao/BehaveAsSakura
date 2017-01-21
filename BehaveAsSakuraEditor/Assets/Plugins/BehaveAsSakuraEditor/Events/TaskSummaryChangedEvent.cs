namespace BehaveAsSakura.Editor
{
    public class TaskSummaryChangedEvent : EditorEvent
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        public TaskSummaryChangedEvent(string id)
            : base(id)
        {
        }
    }
}
