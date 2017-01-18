namespace BehaveAsSakura.Editor
{
    public abstract class EditorState : EditorObject
    {
        public delegate void EventAppliedHandler(EditorState state, EditorEvent e);

        public event EventAppliedHandler OnEventApplied;

        protected EditorState(string id)
            : base(id)
        {
        }

        public virtual void ApplyEvent(EditorEvent e)
        {
            if (OnEventApplied != null)
                OnEventApplied(this, e);
        }
    }
}
