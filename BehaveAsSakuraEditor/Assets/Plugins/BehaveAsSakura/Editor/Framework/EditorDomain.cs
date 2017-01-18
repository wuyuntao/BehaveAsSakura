namespace BehaveAsSakura.Editor
{
    class EditorDomain
    {
        public EditorRepository Repository { get; private set; }

        public EditorCommandHandler CommandHandler { get; private set; }

        public EditorDomain(EditorRepository repository, EditorCommandHandler commandHandler)
        {
            Repository = repository;
            CommandHandler = commandHandler;
            CommandHandler.Domain = this;
        }
    }
}
