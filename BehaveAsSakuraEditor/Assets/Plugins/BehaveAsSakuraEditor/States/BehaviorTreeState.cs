using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeState : EditorState
    {
        public static string GetId()
        {
            return "BehaviorTree";
        }

        public BehaviorTreeAsset Asset { get; set; }

        public uint RootTaskId { get; set; }

        public uint NextTaskId { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public BehaviorTreeState(EditorDomain domain, string id)
            : base(domain, id)
        {
            NextTaskId = 1;
        }

        public override void ApplyEvent(EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                OnTaskCreatedEvent((TaskCreatedEvent)e);
            }
            else if (e is BehaviorTreeSummaryChangedEvent)
            {
                OnBehaviorTreeSummaryChangedEvent((BehaviorTreeSummaryChangedEvent)e);
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

        private void OnBehaviorTreeSummaryChangedEvent(BehaviorTreeSummaryChangedEvent e)
        {
            Title = e.Title;
            Comment = e.Comment;
        }

        public BehaviorTreeDesc BuildDesc()
        {
            var tasks = new List<TaskDescWrapper>();

            if (RootTaskId > 0)
                FindTaskRecursively(tasks, RootTaskId);

            var desc = new BehaviorTreeDesc()
            {
                Title = Title,
                Comment = Comment,
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
