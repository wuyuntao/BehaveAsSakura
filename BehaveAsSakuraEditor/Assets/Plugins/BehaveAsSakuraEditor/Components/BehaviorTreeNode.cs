using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeNode : Node
    {
        public BehaviorTreeNode(EditorDomain domain, BehaviorTreeView parent, string title)
            : base(domain, parent, title, EditorConfiguration.BehaviorTreeNodePosition, EditorConfiguration.BehaviorTreeNodeSize, EditorConfiguration.BehaviorTreeNodeStyle)
        {
        }
    }
}
