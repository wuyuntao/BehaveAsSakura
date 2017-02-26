using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class LeafTaskNode : TaskNode
    {
        public LeafTaskNode(EditorDomain domain, EditorComponent parent, TaskState task)
            : base(domain, parent, task)
        {
        }

        protected override bool CanCreateChildTask()
        {
            return false;
        }
    }
}
