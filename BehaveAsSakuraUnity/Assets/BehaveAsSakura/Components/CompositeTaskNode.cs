using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class CompositeTaskNode : TaskNode
    {
        public CompositeTaskNode(EditorDomain domain, EditorComponent parent, TaskState task)
            : base(domain, parent, task)
        {
        }

        protected override bool CanCreateChildTask()
        {
            return true;
        }
    }
}
