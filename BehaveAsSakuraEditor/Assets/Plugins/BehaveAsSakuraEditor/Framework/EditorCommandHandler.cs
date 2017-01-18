namespace BehaveAsSakura.Editor
{
    public abstract class EditorCommandHandler
    {
        public EditorDomain Domain { get; internal set; }

        public EditorRepository Repository { get { return Domain.Repository; } }

        public virtual void ProcessCommand(EditorCommand command)
        {
            Logger.Debug("Process command '{0}'", command);
        }
    }
}
