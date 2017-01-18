using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class TaskNode : Node
    {
        public TaskState Task { get; private set; }

        protected TaskNode(EditorDomain domain, EditorComponent parent, TaskState task, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent, null, position, size, style)
        {
            Task = task;
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            EditorHelper.AddNewTaskMenuItems(menu, CanCreateChildTask(), (s) => OnContextMenu_NewTask((Type)s, e.mousePosition));
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
