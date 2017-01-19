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
                  , string.Format("{0}-Node", parent.Tree.Id)
                  , I18n._("Root Node")
                  , EditorConfiguration.BehaviorTreeNodePosition
                  , EditorConfiguration.BehaviorTreeNodeSize
                  , EditorConfiguration.BehaviorTreeNodeStyle)
        {
            Tree = parent.Tree;
            Tree.OnEventApplied += Tree_OnEventApplied;
        }

        private void Tree_OnEventApplied(EditorState state, EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                Children.Add(TaskNode.Create(this, (TaskCreatedEvent)e));
            }
            else if (e is TaskNotCreatedEvent)
            {
                EditorHelper.DisplayDialog("Failed to create task", ((TaskNotCreatedEvent)e).Reason);
            }
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
