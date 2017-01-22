using BehaveAsSakura.Serialization;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public sealed class BehaviorTreeAsset : ScriptableObject
    {
        [SerializeField]
        private byte[] bytes;

        public EditorDomain Domain { get; private set; }

        public BehaviorTreeState Tree { get; private set; }

        [MenuItem("Assets/Create/BehaveAsSakura Behavior Tree")]
        private static void CreateNewAsset()
        {
            var asset = CreateInstance<BehaviorTreeAsset>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (!string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path + "/NewBehaviorTree.asset"));
            AssetDatabase.Refresh();
        }

        public void Deserialize()
        {
            if (Domain != null)
                return;

            var repo = new EditorRepository();
            var handler = new BehaviorTreeCommandHandler();
            Domain = new EditorDomain(repo, handler);
            Domain.OnEventApplied += Domain_OnEventApplied;

            var treeId = BehaviorTreeState.GetId();
            Tree = EditorState.CreateInstance<BehaviorTreeState>(Domain, treeId);
            Tree.Asset = this;
            repo.States[treeId] = Tree;

            if (bytes != null && bytes.Length > 0)
            {
                var treeDesc = BehaviorTreeSerializer.DeserializeDesc(bytes);
                Tree.RootTaskId = treeDesc.RootTaskId;

                if (treeDesc.Tasks != null && treeDesc.Tasks.Length > 0)
                {
                    Tree.NextTaskId = treeDesc.Tasks.Max(t => t.Id) + 1;

                    foreach (var taskDesc in treeDesc.Tasks)
                    {
                        var taskId = TaskState.GetId(taskDesc.Id);
                        var task = EditorState.CreateInstance<TaskState>(Domain, taskId);
                        task.Desc = taskDesc;
                        repo.States[task.Id] = task;
                    }
                }
            }
        }

        private void Domain_OnEventApplied(EditorState state, EditorEvent e)
        {
            try
            {
                var desc = Tree.BuildDesc();

                bytes = BehaviorTreeSerializer.SerializeDesc(desc);

                EditorUtility.SetDirty(this);
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to save asset due to error: {0}", ex.Message);
            }
        }
    }
}
