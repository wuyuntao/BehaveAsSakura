using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(TaskDescWrapper), 1)]
    public class LeafTaskDescWrapper : TaskDescWrapper
    {
    }

    public abstract class LeafTask : Task
    {
        protected LeafTask(BehaviorTree tree, Task parentTask, uint id, ITaskDesc description, ITaskProps props = null)
            : base(tree, parentTask, id, description, props)
        { }
    }
}
