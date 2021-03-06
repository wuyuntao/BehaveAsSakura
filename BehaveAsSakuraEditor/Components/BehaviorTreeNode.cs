﻿using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeNode : Node
    {
        public BehaviorTreeState Tree { get; private set; }

        private Texture2D iconTexture;

        public BehaviorTreeNode(EditorDomain domain, BehaviorTreeView parent)
            : base(domain
                  , parent
                  , string.Format("{0}-Node", parent.Tree.Id))
        {
            Tree = parent.Tree;
            Tree.OnEventApplied += Tree_OnEventApplied;

            if (Tree.RootTaskId > 0)
            {
                var task = (TaskState)Repository.States[TaskState.GetId(Tree.RootTaskId)];
                RootView.Children.Add(TaskNode.Create(RootView, task));
            }
        }

        private void Tree_OnEventApplied(EditorState state, EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                RootView.Children.Add(TaskNode.Create(RootView, ((TaskCreatedEvent)e).NewTask));
            }
            else if (e is TaskNotCreatedEvent)
            {
                EditorHelper.DisplayDialog("Failed to create task", ((TaskNotCreatedEvent)e).Reason);
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();

            var nodeRect = CalculateGUIRect();
            GUI.Box(nodeRect, string.Empty, EditorConfiguration.NodeBackgroundStyle);

            var iconRect = EditorConfiguration.NodeIconPosition;
            iconRect.position += nodeRect.position;
            if (!iconTexture)
                iconTexture = EditorHelper.LoadTexture2D(EditorConfiguration.BehaviorTreeNodeIconPath);
            GUI.Box(iconRect, iconTexture, EditorConfiguration.NodeIconStyle);

            var titleRect = EditorConfiguration.NodeTitlePosition;
            titleRect.position += nodeRect.position;
            var title = I18n._("Root");
            GUI.Box(titleRect, title, EditorConfiguration.NodeTitleStyle);

            if (!string.IsNullOrEmpty(Tree.Title))
            {
                var summaryRect = EditorConfiguration.NodeSummaryPosition;
                summaryRect.position += nodeRect.position;
                GUI.Box(summaryRect, Tree.Title, EditorConfiguration.NodeSummaryStyle);
            }
        }

        protected override Rect CalculateGUIRect()
        {
            return new Rect(RootView.ToWindowPosition(EditorConfiguration.BehaviorTreeNodePosition - EditorConfiguration.NodeSize / 2), EditorConfiguration.NodeSize);
        }

        public override void OnSelect(Event e)
        {
            base.OnSelect(e);

            Selection.activeObject = Tree.Asset;
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            EditorHelper.AddNewTaskMenuItems(menu, Tree.RootTaskId == 0, (s) => OnContextMenu_NewTask((Type)s));
            menu.ShowAsContext();

            e.Use();
        }

        private void OnContextMenu_NewTask(Type taskType)
        {
            Domain.CommandHandler.ProcessCommand(new CreateTaskCommand(Tree.Id)
            {
                TaskType = taskType,
            });
        }
    }
}
