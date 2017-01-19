using UnityEditor;

namespace BehaveAsSakura.Editor
{
    [CustomEditor(typeof(TaskState))]
    public class TaskEditor : UnityEditor.Editor
    {
        private TaskState state;

        private bool showBasic = true;
        private bool showCustom = true;

        public void OnEnable()
        {
            state = (TaskState)target;
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

                basic.Name = EditorGUILayout.TextField(I18n._("Name"), basic.Name);
                basic.Comment = EditorGUILayout.TextField(I18n._("Comment"), basic.Comment);
            }

            showCustom = EditorGUILayout.Foldout(showCustom, I18n._(string.Format("Title of task '{0}'", custom.GetType().FullName)));
            if (showCustom)
            {
                //basic.Name = EditorGUILayout.TextField(I18n._("Name"), basic.Name);
            }
        }
    }
}
