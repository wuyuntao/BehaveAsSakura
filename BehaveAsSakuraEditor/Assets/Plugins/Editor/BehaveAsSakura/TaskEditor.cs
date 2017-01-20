using System.Reflection;
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

        private string taskName;
        private string taskComment;

        private PropertyGroup taskDesc;

        public void OnEnable()
        {
            state = (TaskState)target;

            if (state.Desc == null)
                return;

            taskName = state.Desc.Name;
            taskComment = state.Desc.Comment;
            taskDesc = new PropertyGroup(state.Desc.CustomDesc.GetType(), state.Desc.CustomDesc);
        }

        protected override void OnHeaderGUI()
        {
            // TODO A workaround for display icon and name of task
            if (state.Desc != null)
            {
                var icon = Resources.Load(EditorHelper.GetTaskIcon(state.Desc.CustomDesc.GetType())) as Texture2D;
                if (icon == null)
                    icon = (Texture2D)Resources.Load(EditorConfiguration.DefaultTaskIconPath);

                var title = string.Format("{0} #{1}", EditorHelper.GetTaskTitle(state.Desc.CustomDesc.GetType()), state.Desc.Id);

                EditorHelper.HeaderIconAndTitle(state, icon, title);
            }

            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
            if (state.Desc == null)
                return;

            var descType = state.Desc.CustomDesc.GetType();

            EditorHelper.Foldout(ref showBasic, I18n._("Basic"), () =>
            {
                taskName = EditorGUILayout.TextField(I18n._("Name"), taskName);
                taskComment = EditorHelper.TextArea(I18n._("Comment"), taskComment);

                var help = EditorHelper.GetTaskDescription(descType);
                if (!string.IsNullOrEmpty(help))
                    EditorHelper.ReadOnlyTextArea(I18n._("Help"), help);
            });

            EditorHelper.Foldout(ref showCustom, EditorHelper.GetTaskTitle(descType), () =>
            {
                taskDesc.OnGUI();
            });
        }
    }
}
