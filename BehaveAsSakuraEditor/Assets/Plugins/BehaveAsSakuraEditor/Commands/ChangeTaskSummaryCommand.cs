namespace BehaveAsSakura.Editor
{
    public class ChangeTaskSummaryCommand : EditorCommand
    {
        public string Name { get; set; }

        public string Comment { get; set; }

        public ChangeTaskSummaryCommand(string id)
            : base(id)
        {
        }
    }
}
