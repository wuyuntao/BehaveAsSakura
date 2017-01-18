using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    public interface ILeafTaskDesc : ITaskDesc
    {
    }

    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(TaskDescWrapper), 1)]
    public class LeafTaskDescWrapper : TaskDescWrapper
    {
    }

    public abstract class LeafTask : Task
    {
        protected LeafTask(BehaviorTree tree, Task parentTask, uint id, ILeafTaskDesc description, ITaskProps props = null)
            : base(tree, parentTask, id, description, props)
        { }
    }
}
