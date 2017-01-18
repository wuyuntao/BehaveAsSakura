using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeEditorWindow : EditorWindow
    {
        private EditorDomain domain;
        private BehaviorTreeNodeCanvas view;

        [MenuItem("BehaveAsSakura/New Behavior Tree")]
        private static void NewBehaviorTree()
        {
            //I18n.SetLanguage( "zh_CN" );

            var window = GetWindow<BehaviorTreeEditorWindow>();

            window.minSize = EditorSettings.MinWindowSize;
            window.titleContent = new GUIContent(I18n._("Untitled Behavior Tree"));

            var repo = new EditorRepository();

            var treeId = string.Format("{0}-{1}", typeof(BehaviorTreeState).Name, Guid.NewGuid());
            var tree = new BehaviorTreeState(treeId);
            repo.States[treeId] = tree;

            var handler = new BehaviorTreeCommandHandler();

            window.domain = new EditorDomain(repo, handler);
            window.view = new BehaviorTreeNodeCanvas(window.domain);
        }

        private void OnGUI()
        {
            view.OnGUI();
        }
    }
}
