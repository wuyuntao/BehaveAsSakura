namespace BehaveAsSakura.Editor
{
    public abstract class EditorEvent : EditorObject
    {
        protected EditorEvent(string id)
            : base(id)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} of '{1}'", GetType().FullName, Id);
        }
    }
}
