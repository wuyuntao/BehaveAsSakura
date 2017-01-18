namespace BehaveAsSakura.Editor
{
    abstract class Node : EditorComponent
    {
        protected Node(EditorDomain domain, EditorComponent parent)
            : base(domain, parent)
        {
        }
    }
}
