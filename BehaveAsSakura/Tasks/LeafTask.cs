namespace BehaveAsSakura.Tasks
{
	public abstract class LeafTask : Task
    {
        protected LeafTask(BehaviorTree tree, Task parent, TaskDesc description, TaskProps props)
            : base(tree, parent, description, props)
        { }
    }
}
