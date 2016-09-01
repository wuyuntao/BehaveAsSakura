using System;
using BehaveAsSakura.Variables;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class SubTreeTaskDesc : ITaskDesc
    {
        [ProtoMember(1)]
        public VariableDesc SubTreePath { get; set; }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new SubTreeTask(tree, parentTask, id, this);
        }
    }

    [ProtoContract]
    class SubTreeTaskProps : ITaskProps
    {
        [ProtoMember(1)]
        public BehaviorTreeProps SubTree { get; set; }
    }

    class SubTreeTask : LeafTask
    {
        private SubTreeTaskDesc description;
        private BehaviorTree subTree;

        public SubTreeTask(BehaviorTree tree, Task parentTask, uint id, SubTreeTaskDesc description)
            : base(tree, parentTask, id, description, new SubTreeTaskProps())
        {
            this.description = description;

            var variable = new Variable(this.description.SubTreePath);
            var subTreePath = variable.GetString(this);
            subTree = Tree.TreeManager.CreateTree(Owner, subTreePath, this);
        }

        protected override TaskResult OnUpdate()
        {
            subTree.Update();

            var status = subTree.RootTask.LastResult;
            if (status == TaskResult.Running)
                EnqueueForNextUpdate();

            return status;
        }

        protected override void OnAbort()
        {
            subTree.Abort();

            base.OnAbort();
        }

		protected override ITaskProps OnCloneProps()
		{
			var props = (SubTreeTaskProps)base.OnCloneProps();

			props.SubTree = subTree.CreateSnapshot();

			return props;
		}

		protected override void OnRestoreProps(ITaskProps props)
		{
			base.OnRestoreProps( props );

			var subTreeProps = ( (SubTreeTaskProps)props ).SubTree;

			subTree.RestoreSnapshot( subTreeProps );
		}
	}
}
