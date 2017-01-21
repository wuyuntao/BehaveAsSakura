using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeState : EditorState
    {
        public static string GetId()
        {
            return "BehaviorTree";
        }

        public string FilePath { get; private set; }

        public uint RootTaskId { get; private set; }

        public uint NextTaskId { get; set; }

        public BehaviorTreeState()
        {
            NextTaskId = 1;
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

            RootTaskId = e.NewTask.Desc.Id;
            NextTaskId = Math.Max(NextTaskId, e.NewTask.Desc.Id) + 1;

            NodeLayoutHelper.Calculate(this);
        }

        public BehaviorTreeDesc BuildDesc()
        {
            var tasks = new List<TaskDescWrapper>();

            if (RootTaskId > 0)
                FindTaskRecursively(tasks, RootTaskId);

            var desc = new BehaviorTreeDesc()
            {
                RootTaskId = RootTaskId,
                Tasks = tasks.ToArray(),
            };

            desc.Validate();

            return desc;
        }

        private void FindTaskRecursively(List<TaskDescWrapper> tasks, uint taskId)
        {
            var task = (TaskState)Repository.States[TaskState.GetId(taskId)];

            tasks.Add(task.Desc);

            if (task.Desc is DecoratorTaskDescWrapper)
            {
                var desc = (DecoratorTaskDescWrapper)task.Desc;
                if (desc.ChildTaskId > 0)
                    FindTaskRecursively(tasks, desc.ChildTaskId);
            }
            else if (task.Desc is CompositeTaskDescWrapper)
            {
                var desc = (CompositeTaskDescWrapper)task.Desc;
                if (desc.ChildTaskIds.Count > 0)
                {
                    foreach (var id in desc.ChildTaskIds)
                        FindTaskRecursively(tasks, id);
                }
            }
        }
    }
}
