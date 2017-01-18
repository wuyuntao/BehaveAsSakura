using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class DecoratorTaskNode : TaskNode
    {
        protected DecoratorTaskNode(EditorDomain domain, BehaviorTreeView parent, string title, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent, title, position, size, style)
        {
        }
    }
}
