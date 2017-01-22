using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeNode : Node
    {
        public BehaviorTreeState Tree { get; private set; }

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
                Children.Add(TaskNode.Create(this, task));
            }
        }

        private void Tree_OnEventApplied(EditorState state, EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                Children.Add(TaskNode.Create(this, ((TaskCreatedEvent)e).NewTask));
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
            GUI.Box(nodeRect, string.Empty, EditorConfiguration.BehaviorTreeNodeStyle);

            var iconRect = new Rect(nodeRect.position + new Vector2(15, 15), new Vector2(32, 32));
            var iconTexture = (Texture2D)Resources.Load(EditorConfiguration.BehaviorTreeNodeIconPath);
            GUI.Box(iconRect, iconTexture, new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    background = null,
                    textColor = Color.white,
                }
            });

            var titleRect = new Rect(nodeRect.position + new Vector2(15 + 32 + 10, 12), new Vector2(50, EditorGUIUtility.singleLineHeight));
            var title = I18n._("Root");
            GUI.Box(titleRect, title, new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState()
                {
                    background = null,
                    textColor = Color.white,
                }
            });

            if (!string.IsNullOrEmpty(Tree.Title))
            {
                var summaryRect= new Rect(nodeRect.position + new Vector2(15 + 32 + 10, 12 + EditorGUIUtility.singleLineHeight + 5 ), new Vector2(50, EditorGUIUtility.singleLineHeight));
                GUI.Box(summaryRect, Tree.Title, new GUIStyle()
                {
                    normal = new GUIStyleState()
                    {
                        background = null,
                        textColor = Color.white,
                    }
                });
            }
        }

        protected override Rect CalculateGUIRect()
        {
            return new Rect(RootView.ToWindowPosition(EditorConfiguration.BehaviorTreeNodePosition - EditorConfiguration.BehaviorTreeNodeSize / 2), EditorConfiguration.BehaviorTreeNodeSize);
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
