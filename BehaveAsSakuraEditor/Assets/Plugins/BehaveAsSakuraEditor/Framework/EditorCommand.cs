namespace BehaveAsSakura.Editor
{
    public abstract class EditorCommand : EditorObject
    {
        protected EditorCommand(string objectId)
            : base(objectId)
        {
        }
    }
}
