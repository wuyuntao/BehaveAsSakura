﻿using BehaveAsSakura.Tasks;
using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class TaskNode : Node
    {
        public TaskState Task { get; private set; }

        private Texture2D iconTexture;

        protected TaskNode(EditorDomain domain, EditorComponent parent, TaskState task)
            : base(domain
                  , parent
                  , string.Format("{0}-Node", task.Id))
        {
            Task = task;
            Task.OnEventApplied += Task_OnEventApplied;

            if (task.Desc is DecoratorTaskDescWrapper)
            {
                var desc = (DecoratorTaskDescWrapper)task.Desc;

                if (desc.ChildTaskId > 0)
                    CreateChildTaskNode(desc.ChildTaskId);
            }
            else if (task.Desc is CompositeTaskDescWrapper)
            {
                var desc = (CompositeTaskDescWrapper)task.Desc;

                foreach (var id in desc.ChildTaskIds)
                    CreateChildTaskNode(id);
            }
        }

        private void CreateChildTaskNode(uint taskId)
        {
            var task = (TaskState)Repository.States[TaskState.GetId(taskId)];

            RootView.Children.Add(Create(RootView, task));
        }

        public static TaskNode Create(EditorComponent parent, TaskState state)
        {
            if (state.Desc is LeafTaskDescWrapper)
            {
                return new LeafTaskNode(parent.Domain, parent, state);
            }
            else if (state.Desc is DecoratorTaskDescWrapper)
            {
                return new DecoratorTaskNode(parent.Domain, parent, state);
            }
            else if (state.Desc is CompositeTaskDescWrapper)
            {
                return new CompositeTaskNode(parent.Domain, parent, state);
            }
            else
                throw new NotSupportedException(state.Desc.ToString());
        }

        private void Task_OnEventApplied(EditorState state, EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                RootView.Children.Add(Create(RootView, ((TaskCreatedEvent)e).NewTask));
            }
            else if (e is TaskNotCreatedEvent)
            {
                EditorHelper.DisplayDialog("Failed to create task", ((TaskNotCreatedEvent)e).Reason);
            }
            else if (e is TaskRemovedEvent)
            {
                RootView.Children.Remove(this);
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (Task.IsAscendentCollapsed)
                return;

            var descType = Task.Desc.CustomDesc.GetType();

            var nodeRect = CalculateGUIRect();
            GUI.Box(nodeRect, string.Empty, EditorConfiguration.NodeBackgroundStyle);

            var iconRect = EditorConfiguration.NodeIconPosition;
            iconRect.position += nodeRect.position;
            if (!iconTexture)
                iconTexture = EditorHelper.LoadTaskIcon(descType);
            GUI.Box(iconRect, iconTexture, EditorConfiguration.NodeIconStyle);

            var titleRect = EditorConfiguration.NodeTitlePosition;
            titleRect.position += nodeRect.position;
            var title = EditorHelper.GetTaskTitle(descType, false);
            GUI.Box(titleRect, title, EditorConfiguration.NodeTitleStyle);

            var summaryRect = EditorConfiguration.NodeSummaryPosition;
            summaryRect.position += nodeRect.position;
            var summary = string.Format("#{0} {1}", Task.Desc.Id, Task.Desc.Title);
            GUI.Box(summaryRect, summary, EditorConfiguration.NodeSummaryStyle);

            DrawConnection();
        }

        protected override Rect CalculateGUIRect()
        {
            return new Rect(RootView.ToWindowPosition(Task.Position - EditorConfiguration.NodeSize / 2), EditorConfiguration.NodeSize);
        }

        private void DrawConnection()
        {
            var fromPoint = RootView.ToWindowPosition(Task.Position + new Vector2(0, -EditorConfiguration.NodeSize.y / 2 + EditorConfiguration.TaskNodeConnectionPadding));
            Vector2 toPoint;
            if (Task.ParentTask == null)
            {
                toPoint = EditorConfiguration.BehaviorTreeNodePosition + new Vector2(0, EditorConfiguration.NodeSize.y / 2 - EditorConfiguration.TaskNodeConnectionPadding);
            }
            else
            {
                var parentNodeId = string.Format("{0}-Node", Task.ParentTask.Id);
                var parentNode = RootView.Children.Find(n => n.Id == parentNodeId) as TaskNode;

                toPoint = parentNode.Task.Position + new Vector2(0, EditorConfiguration.NodeSize.y / 2 - EditorConfiguration.TaskNodeConnectionPadding);
            }

            toPoint = RootView.ToWindowPosition(toPoint);

            Handles.DrawBezier(fromPoint
                    , toPoint
                    , fromPoint - Vector2.up * EditorConfiguration.TaskNodeConnectionTangent
                    , toPoint - Vector2.down * EditorConfiguration.TaskNodeConnectionTangent
                    , EditorConfiguration.TaskNodeConnectionColor
                    , null
                    , EditorConfiguration.TaskNodeConnectionLineWidth);
        }

        private static void OnTaskNotCreatedEvent(EditorEvent e)
        {
            var title = I18n._("Failed to create task");
            var message = I18n._(((TaskNotCreatedEvent)e).Reason);
            var ok = I18n._("Ok");

            EditorUtility.DisplayDialog(title, message, ok);
        }

        public override void OnSelect(Event e)
        {
            base.OnSelect(e);

            Selection.activeObject = Task.Wrapper;
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            EditorHelper.AddNewTaskMenuItems(menu, CanCreateChildTask(), (s) => OnContextMenu_NewTask((Type)s));

            menu.AddItem(new GUIContent(I18n._("Remove Task")), false, OnContextMenu_RemoveTask);

            EditorHelper.AddMoveTaskMenuItems(menu, Task, (s) => OnContextMenu_MoveTask((int)s));

            EditorHelper.AddCollapseTaskMenuItems(menu, Task, (c) => OnContextMenu_CollapseTask((bool)c));

            menu.ShowAsContext();

            e.Use();
        }

        private void OnContextMenu_NewTask(Type taskType)
        {
            Domain.CommandHandler.ProcessCommand(new CreateTaskCommand(Task.Id) { TaskType = taskType });
        }

        private void OnContextMenu_RemoveTask()
        {
            Domain.CommandHandler.ProcessCommand(new RemoveTaskCommand(Task.Id));
        }

        private void OnContextMenu_MoveTask(int offset)
        {
            Domain.CommandHandler.ProcessCommand(new MoveTaskCommand(Task.Id) { Offset = offset });
        }

        private void OnContextMenu_CollapseTask(bool collapse)
        {
            Domain.CommandHandler.ProcessCommand(new ChangeTaskCollapseCommand(Task.Id) { IsCollapsed = collapse });
        }

        protected abstract bool CanCreateChildTask();
    }
}
