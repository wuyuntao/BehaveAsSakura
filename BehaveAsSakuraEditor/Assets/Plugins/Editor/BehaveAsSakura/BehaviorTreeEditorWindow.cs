using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeEditorWindow : EditorWindow
    {
        private EditorDomain domain;
        private BehaviorTreeView view;

        [MenuItem("BehaveAsSakura/New Behavior Tree")]
        private static void NewBehaviorTree()
        {
            //I18n.SetLanguage( "zh_CN" );

            var window = GetWindow<BehaviorTreeEditorWindow>(I18n._("Untitled Behavior Tree"), typeof(SceneView));

            window.minSize = EditorConfiguration.MinWindowSize;

            var repo = new EditorRepository();
            var handler = new BehaviorTreeCommandHandler();
            var domain = new EditorDomain(repo, handler);

            var treeId = BehaviorTreeState.GetId();
            var tree = EditorState.CreateState<BehaviorTreeState>(domain, treeId);
            repo.States[treeId] = tree;

            window.domain = domain;
            window.view = new BehaviorTreeView(window.domain, tree, window.position.size);
        }

        private void OnGUI()
        {
            if (view == null)
                return;

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    view.OnMouseDown(Event.current);
                    break;

                case EventType.MouseUp:
                    view.OnMouseUp(Event.current);
                    break;

                case EventType.MouseDrag:
                    view.OnMouseDrag(Event.current);
                    break;

                case EventType.MouseMove:
                    view.OnMouseMove(Event.current);
                    break;

                case EventType.Layout:
                    view.OnLayoutChange(Event.current, position);
                    break;
            }

            view.OnGUI();

            //Logger.Debug("OnGUI: {0}", Event.current);
        }
    }
}
