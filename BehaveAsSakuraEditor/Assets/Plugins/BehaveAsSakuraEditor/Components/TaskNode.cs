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
                  , I18n._(string.Format("Title of task '{0}'", task.Desc.CustomDesc.GetType().FullName))
                  , task.Position
                  , EditorConfiguration.TaskNodeSize
                  , EditorConfiguration.TaskNodeStyle)
        {
            Task = task;
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            EditorHelper.AddNewTaskMenuItems(menu, CanCreateChildTask(), (s) => OnContextMenu_NewTask((Type)s, e.mousePosition - RootView.ScrollOffset));
            menu.ShowAsContext();

            e.Use();
        }

        private void OnContextMenu_NewTask(Type taskType, Vector2 taskPosition)
        {
            Domain.CommandHandler.ProcessCommand(new CreateTaskCommand(Task.Id)
            {
                TaskType = taskType,
                TaskPosition = taskPosition,
            });
        }

        protected abstract bool CanCreateChildTask();
    }
}
