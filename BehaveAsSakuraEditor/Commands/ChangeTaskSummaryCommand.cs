namespace BehaveAsSakura.Editor
{
    public class ChangeTaskSummaryCommand : EditorCommand
    {
        public string Title { get; set; }

        public string Comment { get; set; }

        public ChangeTaskSummaryCommand(string id)
            : base(id)
        {
        }
    }
}
