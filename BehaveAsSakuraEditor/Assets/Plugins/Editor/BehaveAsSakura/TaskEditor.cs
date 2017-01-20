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

        public void OnEnable()
        {
            state = (TaskState)target;

            if (state.Desc != null)
            {
                taskName = state.Desc.Name;
                taskComment = state.Desc.Comment;
            }
        }

        public override void OnInspectorGUI()
        {
            if (state.Desc == null)
                return;

            var basic = state.Desc;
            var custom = basic.CustomDesc;

            showBasic = EditorGUILayout.Foldout(showBasic, I18n._("Basic"));
            if (showBasic)
            {
                EditorHelper.ReadOnlyTextField(I18n._("Id"), basic.Id.ToString());

                taskName = EditorGUILayout.TextField(I18n._("Name"), taskName);
                taskComment = EditorHelper.TextArea(I18n._("Comment"), taskComment);
            }

            showCustom = EditorGUILayout.Foldout(showCustom, I18n._(string.Format("Title of task '{0}'", custom.GetType().FullName)));
            if (showCustom)
            {
                var helpText = I18n.__(string.Format("Description of task '{0}'", custom.GetType().FullName));
                if (!string.IsNullOrEmpty(helpText))
                    EditorHelper.ReadOnlyTextArea(I18n._("Description"), helpText);
            }
        }
    }
}
