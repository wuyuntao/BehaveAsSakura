using BehaveAsSakura.Tasks;
using ProtoBuf;

namespace BehaveAsSakura.Tests
{
	[ProtoContract]
	public sealed class DumpLogTaskDesc : ITaskDesc
	{
		[ProtoMember( 1 )]
		public string Text { get; set; }

		Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
		{
			return new DumpLogTask( tree, parentTask, id, this );
		}
	}

	class DumpLogTask : LeafTask
	{
		private DumpLogTaskDesc description;

		public DumpLogTask(BehaviorTree tree, Task parentTask, uint id, DumpLogTaskDesc description)
            : base(tree, parentTask, id, description)
        {
			this.description = description;
		}

		protected override TaskResult OnUpdate()
		{
			LogInfo( description.Text );

			return TaskResult.Success;
		}
	}
}
