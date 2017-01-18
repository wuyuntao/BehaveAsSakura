using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeView : EditorComponent
    {
        public Vector2 ScrollOffset { get; private set; }

        public BehaviorTreeView(EditorDomain domain, BehaviorTreeState tree, Vector2 windowPosition)
            : base(domain, null)
        {
            ScrollOffset = new Vector2(windowPosition.x / 2, EditorConfiguration.BehaviorTreeNodeSize.y);

            Children.Add(new BehaviorTreeNode(domain, this, null));
        }
    }
}
