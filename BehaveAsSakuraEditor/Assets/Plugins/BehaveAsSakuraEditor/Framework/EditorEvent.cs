namespace BehaveAsSakura.Editor
{
    public abstract class EditorEvent : EditorObject
    {
        protected EditorEvent(string id)
            : base(id)
        {
        }
    }
}
