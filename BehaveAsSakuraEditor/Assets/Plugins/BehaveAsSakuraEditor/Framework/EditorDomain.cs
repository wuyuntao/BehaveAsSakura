namespace BehaveAsSakura.Editor
{
    public delegate void EventAppliedHandler(EditorState state, EditorEvent e);

    public class EditorDomain
    {
        public event EventAppliedHandler OnEventApplied;

        public EditorRepository Repository { get; private set; }

        public EditorCommandHandler CommandHandler { get; private set; }

        public EditorDomain(EditorRepository repository, EditorCommandHandler commandHandler)
        {
            Repository = repository;
            CommandHandler = commandHandler;
            CommandHandler.Domain = this;
        }

        internal void EventApplied(EditorState state, EditorEvent e)
        {
            if (OnEventApplied != null)
                OnEventApplied(state, e);
        }
    }
}
