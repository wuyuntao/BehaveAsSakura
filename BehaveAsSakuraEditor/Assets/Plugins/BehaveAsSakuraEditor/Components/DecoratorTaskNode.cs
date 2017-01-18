using BehaveAsSakura.Tasks;

namespace BehaveAsSakura.Editor
{
    public class DecoratorTaskNode : TaskNode
    {
        public DecoratorTaskNode(EditorDomain domain, EditorComponent parent, TaskState task)
            : base(domain, parent, task)
        {
        }

        protected override bool CanCreateChildTask()
        {
            return ((DecoratorTaskDescWrapper)Task.Desc).ChildTaskId == 0;
        }
    }
}
