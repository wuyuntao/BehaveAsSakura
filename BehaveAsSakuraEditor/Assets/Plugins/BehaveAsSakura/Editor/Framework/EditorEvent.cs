namespace BehaveAsSakura.Editor
{
    abstract class EditorEvent : EditorObject
    {
        protected EditorEvent(string id)
            : base(id)
        {
        }
    }
}
