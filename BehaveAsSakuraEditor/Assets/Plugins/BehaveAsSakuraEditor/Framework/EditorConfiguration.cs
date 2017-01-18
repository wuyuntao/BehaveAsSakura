using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public static class EditorConfiguration
    {
        public static readonly Vector2 MinWindowSize = new Vector2(800, 600);

        public static readonly Vector2 BehaviorTreeNodeSize = new Vector2(100, 100);

        public static readonly Vector2 BehaviorTreeNodePosition = new Vector2(0, 0);

        public static readonly GUIStyle BehaviorTreeNodeStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png"),
                textColor = Color.white,
            },
            border = new RectOffset(12, 12, 12, 12),
        };

        public static readonly Vector2 TaskNodeSize = new Vector2(100, 100);

        public static readonly GUIStyle TaskNodeStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png"),
                textColor = Color.white,
            },
            border = new RectOffset(12, 12, 12, 12),
        };

        public static readonly Logger.Level LoggerLevel = Logger.Level.Debug;
    }
}
