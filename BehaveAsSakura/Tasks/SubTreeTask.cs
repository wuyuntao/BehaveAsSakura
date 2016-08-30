using BehaveAsSakura.Variables;
using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class SubTreeTaskDesc : LeafTaskDesc
	{
		[ProtoMember( 1 )]
		public VariableDesc SubTreePath { get; set; }
	}

	[ProtoContract]
	class SubTreeTaskProps : TaskProps
	{
		[ProtoMember( 1 )]
		public BehaviorTreeProps SubTree;

		public SubTreeTaskProps(uint id)
			: base( id )
		{ }
	}

	public sealed class SubTreeTask : LeafTask
	{
		private SubTreeTaskDesc description;
		private BehaviorTree subTree;
		private SubTreeTaskProps props;

		public SubTreeTask(BehaviorTree tree, Task parent, SubTreeTaskDesc description)
			: base( tree, parent, description, new SubTreeTaskProps( description.Id ) )
		{
			this.description = description;
			props = (SubTreeTaskProps)Props;

			var variable = new Variable( this.description.SubTreePath );
			var subTreePath = variable.GetString( this );

			subTree = Tree.TreeManager.CreateTree( Owner, subTreePath, this );
		}

		protected override TaskResult OnUpdate()
		{
			subTree.Update();

			var status = subTree.RootTask.LastResult;
			if( status == TaskResult.Running )
				EnqueueForNextUpdate();

			return status;
		}

		protected override void OnAbort()
		{
			subTree.Abort();

			base.OnAbort();
		}
	}
}
