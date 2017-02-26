namespace BehaveAsSakura.Editor
{
    public abstract class EditorCommand : EditorObject
    {
        protected EditorCommand(string objectId)
            : base(objectId)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} of '{1}'", GetType().FullName, Id);
        }
    }
}
