using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeView : EditorComponent
    {
        private const string backgroundPath = "BehaveAsSakuraEditor/Textures/BehavioreTreeViewBackground";

        private Vector2 size;
        private Vector2 scrollOffset;
        private Texture2D background;

        public BehaviorTreeState Tree { get; private set; }

        public BehaviorTreeView(EditorDomain domain, BehaviorTreeState tree, Vector2 windowSize)
            : base(domain, null, string.Format("{0}-View", tree.Id))
        {
            Tree = tree;
            size = windowSize;
            scrollOffset = new Vector2(windowSize.x / 2, EditorConfiguration.BehaviorTreeNodeSize.y);
            background = (Texture2D)Resources.Load(backgroundPath);

            Children.Add(new BehaviorTreeNode(domain, this));
        }

        public override void OnGUI()
        {
            DrawBackground();

            base.OnGUI();
        }

        private void DrawBackground()
        {
            var width = 1f / background.width;
            var height = 1f / background.height;
            var viewRect = new Rect(0, 0, size.x, size.y);
            var uvMapping = new Rect(-scrollOffset.x * width, (scrollOffset.y - size.y) * height, size.x * width, size.y * height);
            GUI.DrawTextureWithTexCoords(viewRect, background, uvMapping);
        }

        public override bool OnMouseDrag(Event e)
        {
            if (base.OnMouseDrag(e))
                return true;

            scrollOffset += e.delta;
            e.Use();

            return true;
        }

        public override bool OnLayoutChange(Event e, Rect windowPosition)
        {
            size = windowPosition.size;

            return base.OnLayoutChange(e, windowPosition);
        }

        public Vector2 ToWindowPosition(Vector2 pos)
        {
            return pos + scrollOffset;
        }

        public Vector2 ToViewPosition(Vector2 pos)
        {
            return pos - scrollOffset;
        }
    }
}
