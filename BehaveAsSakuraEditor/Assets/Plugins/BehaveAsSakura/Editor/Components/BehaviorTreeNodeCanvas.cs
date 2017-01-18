namespace BehaveAsSakura.Editor
{
    class BehaviorTreeNodeCanvas : EditorComponent
    {
        private BehaviorTreeNode node;

        public BehaviorTreeNodeCanvas(EditorDomain domain)
            : base(domain, null)
        {
            node = new BehaviorTreeNode(domain, this);

            Children.Add(node);
        }
    }
}
