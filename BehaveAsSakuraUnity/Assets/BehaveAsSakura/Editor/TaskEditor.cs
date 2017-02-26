using BehaveAsSakura.Tasks;
using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    [CustomEditor(typeof(TaskAsset))]
    public class TaskEditor : UnityEditor.Editor
    {
        private TaskState state;
        private PropertyGroup view;

        public void OnEnable()
        {
            state = ((TaskAsset)target).State;

            if (state == null || state.Desc == null)
                return;

            var taskDescType = state.Desc.CustomDesc.GetType();
            view = new PropertyGroup(state.Domain, null, taskDescType, EditorHelper.CloneObject(taskDescType, state.Desc.CustomDesc));
        }

        public void OnDisable()
        {
            if (state == null || state.Desc == null)
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
            if (state != null && state.Desc != null)
            {
                var icon = Resources.Load(EditorHelper.GetTaskIcon(state.Desc.CustomDesc.GetType())) as Texture2D;
                if (icon == null)
                    icon = (Texture2D)Resources.Load(EditorConfiguration.DefaultTaskIconPath);

                var title = GetTaskTitle(state.Desc);

                EditorHelper.HeaderIconAndTitle(state.Wrapper, icon, title);
            }

            base.OnHeaderGUI();
        }

        private static string GetTaskTitle(TaskDescWrapper desc)
        {
            var title = string.Format("{0} #{1}", EditorHelper.GetTaskTitle(desc.CustomDesc.GetType()), desc.Id);

            if (!string.IsNullOrEmpty(desc.Title))
                title = string.Format("{0} ({1})", title, desc.Title);

            return title;
        }

        public override void OnInspectorGUI()
        {
            if (state == null || state.Desc == null)
                return;

            var help = EditorHelper.GetTaskDescription(state.Desc.CustomDesc.GetType());
            if (!string.IsNullOrEmpty(help))
            {
                EditorHelper.ReadOnlyTextArea(I18n._("Help"), help);
                EditorGUILayout.Space();
            }

            var newTaskTitle = EditorHelper.TextField(I18n._("Title"), state.Desc.Title);
            var newTaskComment = EditorHelper.TextArea(I18n._("Comment"), state.Desc.Comment);

            if (newTaskTitle != state.Desc.Title || newTaskComment != state.Desc.Comment)
            {
                state.CommandHandler.ProcessCommand(new ChangeTaskSummaryCommand(state.Id)
                {
                    Title = newTaskTitle,
                    Comment = newTaskComment,
                });
            }

            EditorGUILayout.Space();
            view.OnGUI();
            if (view.IsDirty)
            {
                view.CommandHandler.ProcessCommand(new ChangeTaskDescCommand(state.Id)
                {
                    CustomDesc = (ITaskDesc)view.Value,
                });
            }
        }
    }
}
