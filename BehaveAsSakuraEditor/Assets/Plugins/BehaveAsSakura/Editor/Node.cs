namespace BehaveAsSakura.Editor
{
    abstract class Node : EditorComponent
    {
        protected Node(EditorComponent parent)
            : base(parent)
        {
        }
    }
}
