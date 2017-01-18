namespace BehaveAsSakura.Editor
{
    public abstract class EditorState : EditorObject
    {
        public delegate void EventAppliedHandler(EditorState state, EditorEvent e);

        public event EventAppliedHandler OnEventApplied;

        public EditorDomain Domain { get; private set; }

        public EditorRepository Repository { get { return Domain.Repository; } }

        protected EditorState(EditorDomain domain, string id)
            : base(id)
        {
            Domain = domain;
        }

        public virtual void ApplyEvent(EditorEvent e)
        {
            if (OnEventApplied != null)
                OnEventApplied(this, e);
        }
    }
}
