using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaveAsSakura.Tasks
{
    public enum TaskResult : byte
    {
        Running,
        Success,
        Failure,
    }

    public enum TaskState : byte
    {
        Suspend,            // (WaitForUpdate, WaitForAbort) -> Suspend -> WaitForStart
        WaitForStart,       // Suspend -> WaitForStart -> (WaitForUpdate, WaitForAbort)
        WaitForUpdate,      // WaitForStart -> WaitForUpdate -> (WaitForEnqueue, WaitForAbort, Suspend)
        WaitForAbort,       // (WaitForStart, WaitForUpdate, WaitForEnqueue) -> WaitForAbort -> Suspend
        WaitForEnqueue,		// WaitForUpdate -> WaitForEnqueue -> (WaitForUpdate, WaitForAbort)
    }

    [ProtoContract]
    public class TaskDesc
    {
        [ProtoMember(1)]
        public uint Id;
        [ProtoMember(2, IsRequired = false)]
        public string Name { get; set; }
        [ProtoMember(3, IsRequired = false)]
        public string Comment { get; set; }

    }

    [ProtoContract]
    public class TaskProps
    {
        [ProtoMember(1)]
        public uint Id;
        [ProtoMember(2)]
        public TaskState State = TaskState.Suspend;
        [ProtoMember(3)]
        public TaskResult LastResult = TaskResult.Running;
    }

    public abstract class Task
    {
        private BehaviorTree tree;
        private Task parent;
        private TaskDesc description;
        private TaskProps props;

        protected Task(BehaviorTree tree, Task parent, TaskDesc description, TaskProps props)
        {
            this.tree = tree;
            this.parent = parent;
            this.description = description;
            this.props = props;
        }
    }
}
