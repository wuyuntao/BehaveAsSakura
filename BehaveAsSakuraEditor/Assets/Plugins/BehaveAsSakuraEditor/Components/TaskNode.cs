using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class TaskNode : Node
    {
        private uint taskId;

        protected TaskNode(EditorDomain domain, EditorComponent parent, uint taskId, Vector2 position, Vector2 size, GUIStyle style)
            : base(domain, parent, null, position, size, style)
        {
            this.taskId = taskId;
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            EditorHelper.AddNewTaskMenuItems(menu, CanCreateChildTask(), OnContextMenu_NewTask);
            menu.ShowAsContext();

            e.Use();
        }

        protected abstract bool CanCreateChildTask();

        private void OnContextMenu_NewTask(object userData)
        {
            var taskType = (Type)userData;

            Debug.LogFormat("OnContextMenu_NewTask {0}", taskType);
        }

        protected TaskState Task
        {
            get { return (TaskState)Repository.States[TaskState.GetId(taskId)]; }
        }
    }
}
