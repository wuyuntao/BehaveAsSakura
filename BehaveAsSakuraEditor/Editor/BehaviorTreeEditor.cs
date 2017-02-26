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
            //BehaviorTreeSerializer.Initialize(new FlatBuffersSerializer());

            asset = (BehaviorTreeAsset)target;
            asset.Deserialize();

            BehaviorTreeEditorWindow.CreateWindow(asset);
        }

        protected override void OnHeaderGUI()
        {
            if (asset != null && asset.Tree != null)
            {
                // TODO A workaround for display icon and name of task
                var icon = (Texture2D)Resources.Load(EditorConfiguration.BehaviorTreeNodeIconPath);
                var title = asset.name;

                EditorHelper.HeaderIconAndTitle(asset, icon, title);
            }
                
            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
            if (asset == null)
                return;

            var newTaskTitle = EditorHelper.TextField(I18n._("Title"), asset.Tree.Title);
            var newTaskComment = EditorHelper.TextArea(I18n._("Comment"), asset.Tree.Comment);

            if (newTaskTitle != asset.Tree.Title || newTaskComment != asset.Tree.Comment)
            {
                asset.Tree.CommandHandler.ProcessCommand(new ChangeBehaviorTreeSummaryCommand(asset.Tree.Id)
                {
                    Title = newTaskTitle,
                    Comment = newTaskComment,
                });
            }
        }
    }
}
