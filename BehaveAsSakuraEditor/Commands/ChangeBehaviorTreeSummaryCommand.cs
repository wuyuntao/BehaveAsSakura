namespace BehaveAsSakura.Editor
{
    public class ChangeBehaviorTreeSummaryCommand : EditorCommand
    {
        public string Title { get; set; }

        public string Comment { get; set; }

        public ChangeBehaviorTreeSummaryCommand(string id)
            : base(id)
        {
        }
    }
}
