using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    [CustomEditor(typeof(BehaviorTreeAsset))]
    public class BehaviorTreeAssetEditor : UnityEditor.Editor
    {
        private BehaviorTreeAsset asset;

        public void OnEnable()
        {
            asset = (BehaviorTreeAsset)target;
            asset.Deserialize();

            BehaviorTreeEditorWindow.CreateWindow(asset);
        }

        protected override void OnHeaderGUI()
        {
            // TODO A workaround for display icon and name of task
            var icon = (Texture2D)Resources.Load(EditorConfiguration.DefaultBehaviorTreeIconPath);
            var title = I18n._("Behavior Tree");

            EditorHelper.HeaderIconAndTitle(asset, icon, title);

            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
        }
    }
}
