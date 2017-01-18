using BehaveAsSakura.Tasks;
using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeNode : Node
    {
        public BehaviorTreeState Tree { get; private set; }

        public BehaviorTreeNode(EditorDomain domain, BehaviorTreeView parent)
            : base(domain, parent, I18n._("Root Node"), EditorConfiguration.BehaviorTreeNodePosition, EditorConfiguration.BehaviorTreeNodeSize, EditorConfiguration.BehaviorTreeNodeStyle)
        {
            Tree = parent.Tree;
            Tree.OnEventApplied += Tree_OnEventApplied;
        }

        private void Tree_OnEventApplied(EditorState state, EditorEvent e)
        {
            if (e is TaskCreatedEvent)
            {
                OnTaskCreatedEvent((TaskCreatedEvent)e);
            }
            else if (e is TaskNotCreatedEvent)
            {
                OnTaskNotCreatedEvent(e);
            }
        }

        private void OnTaskCreatedEvent(TaskCreatedEvent e)
        {
            TaskNode node;
            if (e.NewTask.Desc is LeafTaskDescWrapper)
            {
                node = new LeafTaskNode(Domain, this, e.NewTask);
            }
            else if (e.NewTask.Desc is DecoratorTaskDescWrapper)
            {
                node = new DecoratorTaskNode(Domain, this, e.NewTask);
            }
            else if (e.NewTask.Desc is CompositeTaskDescWrapper)
            {
                node = new CompositeTaskNode(Domain, this, e.NewTask);
            }
            else
                throw new NotSupportedException(e.NewTask.Desc.ToString());

            Children.Add(node);
        }

        private static void OnTaskNotCreatedEvent(EditorEvent e)
        {
            var title = I18n._("Failed to create task");
            var message = I18n._(((TaskNotCreatedEvent)e).Reason);
            var ok = I18n._("Ok");

            EditorUtility.DisplayDialog(title, message, ok);
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            EditorHelper.AddNewTaskMenuItems(menu, Tree.RootTaskId == 0, (s) => OnContextMenu_NewTask((Type)s, e.mousePosition - RootView.ScrollOffset));
            menu.ShowAsContext();

            e.Use();
        }

        private void OnContextMenu_NewTask(Type taskType, Vector2 taskPosition)
        {
            Domain.CommandHandler.ProcessCommand(new CreateTaskCommand(Tree.Id)
            {
                TaskType = taskType,
                TaskPosition = taskPosition,
            });
        }
    }
}
