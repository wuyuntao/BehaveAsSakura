using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class Node : EditorComponent
    {
        public new BehaviorTreeView Parent { get; private set; }

        public string Title { get; private set; }

        public Vector2 Position { get; private set; }

        public Vector2 Size { get; private set; }

        public GUIStyle Style { get; private set; }

        protected Node(EditorDomain domain, BehaviorTreeView parent, string title, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent)
        {
            Parent = parent;
            Title = title;
            Position = position;
            Size = size;
            Style = style;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            var rect = new Rect(Position + Parent.ScrollOffset - Size / 2, Size);

            GUI.Box(rect, Title, Style);
        }
    }
}
