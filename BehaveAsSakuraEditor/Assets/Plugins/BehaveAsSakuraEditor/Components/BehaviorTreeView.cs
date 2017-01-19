using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeView : EditorComponent
    {
        private Vector2 scrollOffset;

        public BehaviorTreeState Tree { get; private set; }


        public BehaviorTreeView(EditorDomain domain, BehaviorTreeState tree, Vector2 windowPosition)
            : base(domain, null)
        {
            Tree = tree;
            scrollOffset = new Vector2(windowPosition.x / 2, EditorConfiguration.BehaviorTreeNodeSize.y);
            Children.Add(new BehaviorTreeNode(domain, this));
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
