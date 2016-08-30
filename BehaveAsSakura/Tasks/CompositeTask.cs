namespace BehaveAsSakura.Tasks
{
	public abstract class CompositeTask : Task
    {
        protected CompositeTask(BehaviorTree tree, Task parent, TaskDesc description, TaskProps props)
            : base(tree, parent, description, props)
        { }
    }
}
