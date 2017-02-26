using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeView : EditorComponent
    {
        private Vector2 scrollOffset;
        private Texture2D background;

        public BehaviorTreeState Tree { get; private set; }

        public Vector2 Size { get; private set; }

        public BehaviorTreeView(EditorDomain domain, BehaviorTreeState tree, Vector2 windowSize)
            : base(domain, null, string.Format("{0}-View", tree.Id))
        {
            Tree = tree;
            Size = windowSize;
            scrollOffset = new Vector2(windowSize.x / 2, EditorConfiguration.NodeSize.y);

            Children.Add(new BehaviorTreeNode(domain, this));

            NodeLayoutHelper.Calculate(Tree);
        }

        public override void OnGUI()
        {
            GUI.depth = EditorConfiguration.BehaviorTreeBackgroundDepth;

            DrawBackground();

            base.OnGUI();
        }

        private void DrawBackground()
        {
            // Try to reload background because texture could be unloaded when player stopped
            if (!background)
                background = EditorHelper.LoadTexture2D(EditorConfiguration.BehaviorTreeBackgroundPath);

            var width = 1f / background.width;
            var height = 1f / background.height;
            var viewRect = new Rect(0, 0, Size.x, Size.y);
            var uvMapping = new Rect(-scrollOffset.x * width, (scrollOffset.y - Size.y) * height, Size.x * width, Size.y * height);

            GUI.DrawTextureWithTexCoords(viewRect, background, uvMapping);
        }

        public override bool OnMouseDrag(Event e)
        {
            if (base.OnMouseDrag(e))
                return true;

            if (EditorHelper.IsMiddleButton(e))
            {
                scrollOffset += e.delta;
                e.Use();

                return true;
            }

            return false;
        }

        public override bool OnLayoutChange(Event e, Rect windowPosition)
        {
            Size = windowPosition.size;

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
