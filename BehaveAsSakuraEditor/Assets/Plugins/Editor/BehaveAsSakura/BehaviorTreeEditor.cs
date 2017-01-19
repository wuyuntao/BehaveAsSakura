using UnityEditor;

namespace BehaveAsSakura.Editor
{
    [CustomEditor(typeof(BehaviorTreeState))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        private BehaviorTreeState state;

        private bool showBasic = true;

        public void OnEnable()
        {
            state = (BehaviorTreeState)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();

            showBasic = EditorGUILayout.Foldout(showBasic, I18n._("Basic"));
            if (showBasic)
            {
                EditorHelper.ReadOnlyTextField(I18n._("Id"), state.Id);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
