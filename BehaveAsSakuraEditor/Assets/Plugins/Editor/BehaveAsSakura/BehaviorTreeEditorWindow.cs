using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeEditorWindow : EditorWindow
    {
        [MenuItem("BehaveAsSakura/New Behavior Tree")]
        private static void NewBehaviorTree()
        {
            //I18n.SetLanguage( "zh_CN" );

            var window = GetWindow(typeof(BehaviorTreeEditorWindow));

            window.titleContent = new GUIContent(I18n._("Untitled Behavior Tree"));
        }

        void OnGUI()
        {
            // The actual window code goes here
        }
    }
}
