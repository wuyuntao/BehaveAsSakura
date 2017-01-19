using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class PropertyGridView : EditorComponent
    {
        public BehaviorTreeView RootView { get; private set; }

        public PropertyGridView(EditorDomain domain, BehaviorTreeView parent)
            : base(domain, parent, string.Format("{0}-PropertyGridView", parent.Tree.Id))
        {
            RootView = parent;
        }

        public override void OnGUI()
        {
            GUI.depth = EditorConfiguration.PropertyGridViewDepth;

            //GUILayout.BeginArea(new Rect(RootView.Size.x - EditorConfiguration.PropertyGridViewWidth, 0, EditorConfiguration.PropertyGridViewWidth, RootView.Size.y));
            //GUILayout.BeginScrollView(Vector2.zero, GUIStyle.none);
            //GUILayout.BeginVertical();

            base.OnGUI();

            //GUILayout.EndVertical();
            //GUILayout.EndScrollView();
            //GUILayout.EndArea();
        }
    }
}
