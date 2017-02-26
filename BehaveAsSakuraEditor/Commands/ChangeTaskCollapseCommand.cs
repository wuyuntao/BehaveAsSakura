namespace BehaveAsSakura.Editor
{
    public class ChangeTaskCollapseCommand : EditorCommand
    {
        public bool IsCollapsed { get; set; }

        public ChangeTaskCollapseCommand(string id)
            : base(id)
        {
        }
    }
}
