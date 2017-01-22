using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeEditorWindow : EditorWindow
    {
        private static BehaviorTreeEditorWindow current;

        private BehaviorTreeAsset asset;
        private EditorDomain domain;
        private BehaviorTreeView view;

        public static void CreateWindow(BehaviorTreeAsset asset)
        {
            if (current != null && current.asset == asset)
                return;

            current = GetWindow<BehaviorTreeEditorWindow>(I18n._("BehaveAsSakura Editor"), typeof(SceneView));

            current.minSize = EditorConfiguration.MinWindowSize;
            current.asset = asset;
            current.domain = asset.Domain;
            current.view = new BehaviorTreeView(current.domain, asset.Tree, current.position.size);

            return;
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
