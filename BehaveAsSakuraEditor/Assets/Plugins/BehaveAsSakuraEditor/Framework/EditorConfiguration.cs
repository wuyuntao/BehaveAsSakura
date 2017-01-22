using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public static class EditorConfiguration
    {
        public static readonly Vector2 MinWindowSize = new Vector2(400, 300);

        public static readonly string BehaviorTreeBackgroundPath = "BehaveAsSakuraEditor/Textures/BehavioreTreeViewBackground";

        public static readonly int BehaviorTreeBackgroundDepth = 100;

        public static readonly Vector2 BehaviorTreeNodeSize = new Vector2(140, 64);

        public static readonly Vector2 BehaviorTreeNodePosition = new Vector2(0, 0);

        public static readonly int BehaviorTreeNodeDepth = 90;

        public static readonly string BehaviorTreeNodeIconPath = "BehaveAsSakuraEditor/Textures/Root";

        public static readonly GUIStyle BehaviorTreeNodeStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png"),
                textColor = Color.white,
            },
            border = new RectOffset(12, 12, 12, 12),
        };

        public static readonly Vector2 TaskNodeSize = new Vector2(140, 64);

        public static readonly GUIStyle TaskNodeStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = (Texture2D)EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png"),
                textColor = Color.white,
            },
            border = new RectOffset(12, 12, 12, 12),
        };

        public static readonly Vector2 TaskNodeMinSpace = new Vector2(30, 30);

        public static readonly Logger.Level LoggerLevel = Logger.Level.Debug;

        public static readonly float TaskNodeConnectionPadding = 8;

        public static readonly float TaskNodeConnectionTangent = 30;

        public static readonly Color TaskNodeConnectionColor = Color.white;

        public static readonly float TaskNodeConnectionLineWidth = 3;

        public static readonly string TranslationPath = "BehaveAsSakuraEditor/Translations";

        public static readonly string Language = "en";

        public static readonly int PropertyGridViewDepth = 80;

        public static readonly int PropertyGridViewWidth = 400;

        public static readonly string TaskIconPath = "BehaveAsSakuraEditor/Textures";

        public static readonly string DefaultTaskIconPath = "BehaveAsSakuraEditor/Textures/Default";
    }
}
