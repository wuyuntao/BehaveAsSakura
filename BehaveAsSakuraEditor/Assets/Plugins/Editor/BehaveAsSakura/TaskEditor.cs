using BehaveAsSakura.Tasks;
using System.Reflection;
using UnityEditor;

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

        public override void OnInspectorGUI()
        {
            if (state.Desc == null)
                return;

            var basic = state.Desc;
            var custom = basic.CustomDesc;

            EditorHelper.Foldout(ref showBasic, I18n._("Basic"), () =>
            {
                EditorHelper.ReadOnlyTextField(I18n._("Id"), basic.Id.ToString());

                taskName = EditorGUILayout.TextField(I18n._("Name"), taskName);
                taskComment = EditorHelper.TextArea(I18n._("Comment"), taskComment);
            });

            EditorHelper.Foldout(ref showCustom, EditorHelper.GetTaskTitle(custom.GetType()), () =>
            {
                var helpText = EditorHelper.GetTaskDescription(custom.GetType());
                if (!string.IsNullOrEmpty(helpText))
                    EditorHelper.ReadOnlyTextArea(I18n._("Description"), helpText);

                taskDesc.OnGUI();
            });
        }
    }
}
