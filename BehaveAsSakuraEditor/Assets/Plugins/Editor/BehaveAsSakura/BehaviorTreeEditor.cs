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
            EditorHelper.Foldout(ref showBasic, I18n._("Basic"), () =>
            {
                EditorHelper.ReadOnlyTextField(I18n._("Id"), state.Id);
            });
        }
    }
}
