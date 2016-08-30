namespace BehaveAsSakura.Tasks
{
	public abstract class DecoratorTask : Task
    {
        protected DecoratorTask(BehaviorTree tree, Task parent, TaskDesc description, TaskProps props)
            : base(tree, parent, description, props)
        { }
    }
}
