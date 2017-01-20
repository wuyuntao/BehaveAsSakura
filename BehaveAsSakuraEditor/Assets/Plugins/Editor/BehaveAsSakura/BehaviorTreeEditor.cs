using UnityEditor;
using UnityEngine;

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

        protected override void OnHeaderGUI()
        {
            // TODO A workaround for display icon and name of task
            var icon = (Texture2D)Resources.Load(EditorConfiguration.DefaultBehaviorTreeIconPath);
            var title = I18n._("Behavior Tree");

            EditorHelper.HeaderIconAndTitle(state, icon, title);

            base.OnHeaderGUI();
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
