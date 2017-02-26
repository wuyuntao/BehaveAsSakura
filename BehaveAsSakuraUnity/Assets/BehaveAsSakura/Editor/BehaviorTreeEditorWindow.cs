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

        private Texture2D background;

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
            {
                DrawBackground();
                return;
            }

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

        private void DrawBackground()
        {
            if (!background)
                background = EditorHelper.LoadTexture2D(EditorConfiguration.BehaviorTreeBackgroundPath);

            var size = position.size;
            var width = 1f / background.width;
            var height = 1f / background.height;
            var viewRect = new Rect(0, 0, size.x, size.y);
            var uvMapping = new Rect(0, -size.y * height, size.x * width, size.y * height);

            GUI.DrawTextureWithTexCoords(viewRect, background, uvMapping);
        }
    }
}
