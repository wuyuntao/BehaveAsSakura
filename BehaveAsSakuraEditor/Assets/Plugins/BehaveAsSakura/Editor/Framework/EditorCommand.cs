namespace BehaveAsSakura.Editor
{
    abstract class EditorCommand : EditorObject
    {
        protected EditorCommand(string objectId)
            : base(objectId)
        {
        }
    }
}
