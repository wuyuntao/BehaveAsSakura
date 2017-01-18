using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class CompositeTaskNode : TaskNode
    {
        public CompositeTaskNode(EditorDomain domain, EditorComponent parent, TaskState task, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent, task, position, size, style)
        {
        }

        protected override bool CanCreateChildTask()
        {
            return true;
        }
    }
}
