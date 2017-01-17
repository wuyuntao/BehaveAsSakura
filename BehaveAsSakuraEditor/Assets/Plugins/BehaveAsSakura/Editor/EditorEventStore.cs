namespace BehaveAsSakura.Editor
{
    abstract class EditorEventStore<TSnapshot>
        where TSnapshot : class
    {
        public delegate void EventAppliedHandler(TSnapshot snapshot, EditorEvent e);

        public event EventAppliedHandler OnEventApplied;

        public readonly TSnapshot Snapshot;

        protected EditorEventStore(TSnapshot snapshot)
        {
            Snapshot = snapshot;
        }

        public virtual void ApplyEvent(EditorEvent e)
        {
            if (OnEventApplied != null)
                OnEventApplied(Snapshot, e);
        }
    }
}
