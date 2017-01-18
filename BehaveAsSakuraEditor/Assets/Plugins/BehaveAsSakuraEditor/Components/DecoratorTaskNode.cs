using BehaveAsSakura.Tasks;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class DecoratorTaskNode : TaskNode
    {
        protected DecoratorTaskNode(EditorDomain domain, EditorComponent parent, TaskState task, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent, task, position, size, style)
        {
        }

        protected override bool CanCreateChildTask()
        {
            return ((DecoratorTaskDescWrapper)Task.Desc).ChildTaskId == 0;
        }
    }
}
