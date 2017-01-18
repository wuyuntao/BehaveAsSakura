namespace BehaveAsSakura.Editor
{
    public  abstract class EditorCommandHandler
    {
        public EditorDomain Domain { get; internal set; }

        public EditorRepository Repository { get { return Domain.Repository; } }

        public abstract void ProcessCommand(EditorCommand command);
    }
}
