using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class LeafTaskDesc : TaskDesc
	{
	}

	public abstract class LeafTask : Task
    {
        protected LeafTask(BehaviorTree tree, Task parent, LeafTaskDesc description, TaskProps props)
            : base(tree, parent, description, props)
        { }
    }
}
