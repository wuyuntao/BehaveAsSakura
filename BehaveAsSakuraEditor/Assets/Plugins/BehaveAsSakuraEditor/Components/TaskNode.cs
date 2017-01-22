using BehaveAsSakura.Tasks;
using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class TaskNode : Node
    {
        public TaskState Task { get; private set; }

        protected TaskNode(EditorDomain domain, EditorComponent parent, TaskState task)
            : base(domain
                  , parent
                  , string.Format("{0}-Node", task.Id)
                  , EditorHelper.GetTaskTitle(task.Desc.CustomDesc.GetType())
                  , task.Position
                  , EditorConfiguration.TaskNodeSize
                  , EditorConfiguration.TaskNodeStyle)
        {
            Task = task;
            Task.OnEventApplied += Task_OnEventApplied;

            if (task.Desc is DecoratorTaskDescWrapper)
            {
                var desc = (DecoratorTaskDescWrapper)task.Desc;

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

            Children.Add(Create(this, task));
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
                Children.Add(Create(this, ((TaskCreatedEvent)e).NewTask));
            }
            else if (e is TaskNotCreatedEvent)
            {
                EditorHelper.DisplayDialog("Failed to create task", ((TaskNotCreatedEvent)e).Reason);
            }
        }

        public override void OnGUI()
        {
            Position = Task.Position;

            base.OnGUI();

            DrawConnection();
        }

        private void DrawConnection()
        {
            var fromPoint = RootView.ToWindowPosition(Position + new Vector2(0, -EditorConfiguration.TaskNodeSize.y / 2 + EditorConfiguration.TaskNodeConnectionPadding));
            Vector2 toPoint;
            if (Parent is BehaviorTreeNode)
            {
                var parent = (BehaviorTreeNode)Parent;

                toPoint = parent.Position + new Vector2(0, EditorConfiguration.BehaviorTreeNodeSize.y / 2 - EditorConfiguration.TaskNodeConnectionPadding);
            }
            else if (Parent is TaskNode)
            {
                var parent = (TaskNode)Parent;

                toPoint = parent.Position + new Vector2(0, EditorConfiguration.TaskNodeSize.y / 2 - EditorConfiguration.TaskNodeConnectionPadding);
            }
            else
                throw new NotSupportedException(Parent.ToString());

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

            Selection.activeObject = Task;
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            EditorHelper.AddNewTaskMenuItems(menu, CanCreateChildTask(), (s) => OnContextMenu_NewTask((Type)s));
            menu.ShowAsContext();

            e.Use();
        }

        private void OnContextMenu_NewTask(Type taskType)
        {
            Domain.CommandHandler.ProcessCommand(new CreateTaskCommand(Task.Id)
            {
                TaskType = taskType,
            });
        }

        protected abstract bool CanCreateChildTask();
    }
}
