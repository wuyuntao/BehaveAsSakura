using BehaveAsSakura.Editor;
using BehaveAsSakura.Serialization;
using BehaveAsSakura.Tasks;
using BehaveAsSakura.Variables;
using UnityEngine;

namespace BehaveAsSakura.Examples
{
    public class BasicBehaviorTreeAgent : MonoBehaviour, IBehaviorTreeManagerOwner, IBehaviorTreeOwner
    {
        [SerializeField]
        private BehaviorTreeAsset asset;

        private BehaviorTreeManager treeManager;
        private BehaviorTree tree;

        private void Awake()
        {
            BehaviorTreeSerializer.Initialize(new FlatBuffersSerializer());

            treeManager = new BehaviorTreeManager(this);

            if (asset == null)
                return;

            var treeDesc = BehaviorTreeSerializer.DeserializeDesc(asset.bytes);
            tree = treeManager.CreateTree(this, treeDesc);
        }

        private void FixedUpdate()
        {
            if (tree != null)
            {
                tree.Update();

                if (tree.RootTask.LastResult != TaskResult.Running)
                {
                    Editor.Logger.Info("Behavior Tree ended with {0}", tree.RootTask.LastResult);

                    tree = null;

                    Destroy(gameObject);
                }
            }
        }

        private void OnDestroy()
        {
            if (tree != null)
            {
                tree.Abort();

                tree = null;
            }
        }

        object IVariableContainer.GetValue(string key)
        {
            switch (key)
            {
                case "FirstWaitTime":
                    return 3000u;

                case "SecondWaitTime":
                    return 1500u;

                default:
                    return null;
            }
        }

        BehaviorTreeDesc IBehaviorTreeLoader.LoadTree(string path)
        {
            var asset = Resources.Load(path) as BehaviorTreeAsset;
            if (asset == null)
                return null;

            return BehaviorTreeSerializer.DeserializeDesc(asset.bytes);
        }

        uint IBehaviorTreeOwner.CurrentTime
        {
            get { return (uint)Mathf.RoundToInt(Time.time * 1000); }
        }

        void ILogger.LogDebug(string msg, params object[] args)
        {
            //Editor.Logger.Debug(msg, args);
        }

        void ILogger.LogInfo(string msg, params object[] args)
        {
            Editor.Logger.Info(msg, args);
        }

        void ILogger.LogWarning(string msg, params object[] args)
        {
            Editor.Logger.Warn(msg, args);
        }

        void ILogger.LogError(string msg, params object[] args)
        {
            Editor.Logger.Error(msg, args);
        }
    }
}
