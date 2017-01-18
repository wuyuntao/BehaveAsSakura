using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class DecoratorTaskNode : TaskNode
    {
        protected DecoratorTaskNode(EditorDomain domain, EditorComponent parent, uint taskId, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent, taskId, position, size, style)
        {
        }

        protected override bool CanCreateChildTask()
        {
            return Task.ChildTaskIds.Count == 0;
        }
    }
}
