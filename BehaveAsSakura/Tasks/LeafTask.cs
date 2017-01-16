namespace BehaveAsSakura.Tasks
{
    public abstract class LeafTask : Task
    {
        protected LeafTask(BehaviorTree tree, Task parentTask, uint id, ITaskDesc description, ITaskProps props = null)
            : base(tree, parentTask, id, description, props)
        { }
    }
}
