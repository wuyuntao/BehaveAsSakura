using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class Node : EditorComponent
    {
        public BehaviorTreeView RootView { get; private set; }

        public string Title { get; protected set; }

        public Vector2 Position { get; protected set; }

        public Vector2 Size { get; protected set; }

        public GUIStyle Style { get; protected set; }

        protected Node(EditorDomain domain, EditorComponent parent, string id, string title, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent, id)
        {
            Title = title;
            Position = position;
            Size = size;
            Style = style;
            RootView = FindRootView();
        }

        private BehaviorTreeView FindRootView()
        {
            for (EditorComponent e = this; e != null; e = e.Parent)
            {
                if (e is BehaviorTreeView)
                    return (BehaviorTreeView)e;
            }

            throw new InvalidOperationException("Failed to find root view");
        }

        public override void OnGUI()
        {
            base.OnGUI();

            GUI.Box(CalculateGUIRect(), Title, Style);
        }

        public override bool OnMouseUp(Event e)
        {
            if (base.OnMouseUp(e))
                return true;

            var rect = CalculateGUIRect();
            if (rect.Contains(e.mousePosition))
            {
                if (EditorHelper.IsRightButton(e))
                {
                    OnContextMenu(e);
                    return true;
                }
            }

            return false;
        }

        public virtual void OnContextMenu(Event e)
        {
            Debug.LogFormat("OnContextMenu");
        }

        private Rect CalculateGUIRect()
        {
            return new Rect(RootView.ToWindowPosition(Position - Size / 2), Size);
        }
    }
}
