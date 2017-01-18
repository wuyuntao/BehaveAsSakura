using BehaveAsSakura.Tasks;
using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class TaskState : EditorState
    {
        public static string GetId(uint taskId)
        {
            return string.Format("Task-{0}", taskId);
        }

        public TaskDescWrapper Desc { get; set; }

        public Vector2 Position { get; set; }

        public TaskState(EditorDomain domain, string id, TaskDescWrapper taskDesc)
            : base(domain, id)
        {
            Desc = taskDesc;
        }

        public override void ApplyEvent(EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                OnTaskCreatedEvent((TaskCreatedEvent)e);
            }
            else if (e is TaskRemovedEvent)
            {
            }

            base.ApplyEvent(e);
        }

        private void OnTaskCreatedEvent(TaskCreatedEvent e)
        {
            Repository.States[e.NewTask.Id] = e.NewTask;

            if (Desc is DecoratorTaskDescWrapper)
            {
                var desc = (DecoratorTaskDescWrapper)Desc;
                desc.ChildTaskId = e.NewTask.Desc.Id;
            }
            else if (Desc is CompositeTaskDescWrapper)
            {
                var desc = (CompositeTaskDescWrapper)Desc;
                desc.ChildTaskIds.Add(e.NewTask.Desc.Id);
            }

            var tree = (BehaviorTreeState)Repository.States[BehaviorTreeState.GetId()];
            tree.NextTaskId = Math.Max(tree.NextTaskId, e.NewTask.Desc.Id) + 1;
        }
    }
}
