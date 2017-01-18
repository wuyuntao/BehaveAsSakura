using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class TaskNode : Node
    {
        protected TaskNode(EditorDomain domain, EditorComponent parent, string title, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent, title, position, size, style)
        {
        }
    }
}
