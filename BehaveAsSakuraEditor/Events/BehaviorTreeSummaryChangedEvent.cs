namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeSummaryChangedEvent : EditorEvent
    {
        public string Title { get; set; }

        public string Comment { get; set; }

        public BehaviorTreeSummaryChangedEvent(string id)
            : base(id)
        {
        }
    }
}
