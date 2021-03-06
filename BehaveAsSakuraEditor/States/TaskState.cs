﻿using BehaveAsSakura.Tasks;
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

        public uint ParentTaskId { get; set; }

        public TaskState ParentTask
        {
            get
            {
                if (ParentTaskId == 0)
                    return null;

                return (TaskState)Repository.States[GetId(ParentTaskId)];
            }
        }

        public TaskDescWrapper Desc { get; set; }

        public Vector2 Position { get; set; }

        public bool IsCollapsed { get; set; }

        private TaskAsset wrapper;

        public TaskAsset Wrapper
        {
            get
            {
                if (wrapper == null)
                {
                    wrapper = ScriptableObject.CreateInstance<TaskAsset>();
                    wrapper.State = this;
                }

                return wrapper;
            }
        }

        public BehaviorTreeState Tree
        {
            get { return (BehaviorTreeState)Repository.States[BehaviorTreeState.GetId()]; }
        }

        public TaskState(EditorDomain domain, string id)
            : base(domain, id)
        {
        }

        public override void ApplyEvent(EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                OnTaskCreatedEvent((TaskCreatedEvent)e);
            }
            else if (e is TaskRemovedEvent)
            {
                OnTaskRemovedEvent((TaskRemovedEvent)e);
            }
            else if (e is TaskSummaryChangedEvent)
            {
                OnTaskSummaryChangedEvent((TaskSummaryChangedEvent)e);
            }
            else if (e is TaskPropertyDescEvent)
            {
                OnTaskPropertyChangedEvent((TaskPropertyDescEvent)e);
            }
            else if (e is TaskMovedEvent)
            {
                OnTaskMovedEvent((TaskMovedEvent)e);
            }
            else if (e is TaskCollapseChangedEvent)
            {
                OnTaskCollapseChangedEvent((TaskCollapseChangedEvent)e);
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
            NodeLayoutHelper.Calculate(tree);
        }

        private void OnTaskRemovedEvent(TaskRemovedEvent e)
        {
            var parent = Repository.States[GetId(ParentTaskId)];
            if (parent is BehaviorTreeState)
            {
                var s = (BehaviorTreeState)parent;
                s.RootTaskId = 0;
            }
            else if (parent is TaskState)
            {
                var desc = ((TaskState)parent).Desc;
                if (desc is DecoratorTaskDescWrapper)
                {
                    var d = (DecoratorTaskDescWrapper)desc;
                    d.ChildTaskId = 0;
                }
                else if (desc is CompositeTaskDescWrapper)
                {
                    var d = (CompositeTaskDescWrapper)desc;
                    d.ChildTaskIds.Remove(Desc.Id);
                }
            }

            Repository.States.Remove(Id);

            if (Desc is DecoratorTaskDescWrapper)
            {
                var d = (DecoratorTaskDescWrapper)Desc;
                if (d.ChildTaskId > 0)
                {
                    var taskId = GetId(d.ChildTaskId);
                    Repository.States[taskId].ApplyEvent(new TaskRemovedEvent(taskId));
                }
            }
            else if (Desc is CompositeTaskDescWrapper)
            {
                var d = (CompositeTaskDescWrapper)Desc;

                foreach (var childTaskId in d.ChildTaskIds)
                {
                    var taskId = GetId(childTaskId);
                    Repository.States[taskId].ApplyEvent(new TaskRemovedEvent(taskId));
                }
            }

            NodeLayoutHelper.Calculate(Tree);
        }

        private void OnTaskSummaryChangedEvent(TaskSummaryChangedEvent e)
        {
            Desc.Title = e.Title;
            Desc.Comment = e.Comment;
        }

        private void OnTaskPropertyChangedEvent(TaskPropertyDescEvent e)
        {
            Desc.CustomDesc = e.CustomDesc;
        }

        private void OnTaskMovedEvent(TaskMovedEvent e)
        {
            var parentTaskDesc = (CompositeTaskDescWrapper)ParentTask.Desc;
            var index = parentTaskDesc.ChildTaskIds.IndexOf(Desc.Id);
            var moveIndex = index + e.Offset;

            parentTaskDesc.ChildTaskIds.RemoveAt(index);
            parentTaskDesc.ChildTaskIds.Insert(moveIndex, Desc.Id);

            NodeLayoutHelper.Calculate(Tree);
        }

        private void OnTaskCollapseChangedEvent(TaskCollapseChangedEvent e)
        {
            IsCollapsed = e.IsCollapsed;

            NodeLayoutHelper.Calculate(Tree);
        }

        public bool IsAscendentCollapsed
        {
            get
            {
                if (ParentTask == null)
                    return false;

                if (ParentTask.IsCollapsed)
                    return true;

                return ParentTask.IsAscendentCollapsed;
            }
        }
    }
}
