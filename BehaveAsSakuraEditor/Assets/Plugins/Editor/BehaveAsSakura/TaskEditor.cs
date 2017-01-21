using BehaveAsSakura.Tasks;
using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    [CustomEditor(typeof(TaskState))]
    public class TaskEditor : UnityEditor.Editor
    {
        private TaskState state;

        private bool showBasic = true;
        private bool showCustom = true;

        private PropertyGroup taskDesc;

        public void OnEnable()
        {
            state = (TaskState)target;

            if (state.Desc == null)
                return;

            var taskDescType = state.Desc.CustomDesc.GetType();
            taskDesc = new PropertyGroup(state.Domain, null, taskDescType, EditorHelper.CloneObject(taskDescType, state.Desc.CustomDesc));
        }

        public void OnDisable()
        {
            if (state.Desc == null)
                return;

            try
            {
                state.Desc.CustomDesc.Validate();
            }
            catch (Exception ex)
            {
                Logger.Error("Task '{0}' contains error: {1}", GetTaskTitle(state.Desc), ex);
            }
        }

        protected override void OnHeaderGUI()
        {
            // TODO A workaround for display icon and name of task
            if (state.Desc != null)
            {
                var icon = Resources.Load(EditorHelper.GetTaskIcon(state.Desc.CustomDesc.GetType())) as Texture2D;
                if (icon == null)
                    icon = (Texture2D)Resources.Load(EditorConfiguration.DefaultTaskIconPath);

                var title = GetTaskTitle(state.Desc);

                EditorHelper.HeaderIconAndTitle(state, icon, title);
            }

            base.OnHeaderGUI();
        }

        private static string GetTaskTitle(TaskDescWrapper desc)
        {
            var title = string.Format("{0} #{1}", EditorHelper.GetTaskTitle(desc.CustomDesc.GetType()), desc.Id);

            if (!string.IsNullOrEmpty(desc.Name))
                title = string.Format("{0} ({1})", title, desc.Name);

            return title;
        }

        public override void OnInspectorGUI()
        {
            if (state.Desc == null)
                return;

            var descType = state.Desc.CustomDesc.GetType();

            EditorHelper.Foldout(ref showBasic, I18n._("Basic"), () =>
            {
                var newTaskName = EditorHelper.TextField(I18n._("Name"), state.Desc.Name);
                var newTaskComment = EditorHelper.TextArea(I18n._("Comment"), state.Desc.Comment);

                if (newTaskName != state.Desc.Name || newTaskComment != state.Desc.Comment)
                {
                    state.CommandHandler.ProcessCommand(new ChangeTaskSummaryCommand(state.Id)
                    {
                        Name = newTaskName,
                        Comment = newTaskComment,
                    });
                }

                var help = EditorHelper.GetTaskDescription(descType);
                if (!string.IsNullOrEmpty(help))
                    EditorHelper.ReadOnlyTextArea(I18n._("Help"), help);
            });

            EditorHelper.Foldout(ref showCustom, EditorHelper.GetTaskTitle(descType), () =>
            {
                taskDesc.OnGUI();
                if (taskDesc.IsDirty)
                {
                    taskDesc.CommandHandler.ProcessCommand(new ChangeTaskDescCommand(state.Id)
                    {
                        CustomDesc = (ITaskDesc)taskDesc.Value,
                    });
                }
            });
        }
    }
}
