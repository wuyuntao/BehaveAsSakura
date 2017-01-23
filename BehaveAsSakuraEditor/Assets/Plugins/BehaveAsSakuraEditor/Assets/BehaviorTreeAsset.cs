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
            var domain = new EditorDomain(repo, handler);
            domain.OnEventApplied += Domain_OnEventApplied;

            var treeId = BehaviorTreeState.GetId();
            var tree = new BehaviorTreeState(domain, treeId);
            tree.Asset = this;
            repo.States[treeId] = tree;

            if (bytes != null && bytes.Length > 0)
            {
                var treeDesc = BehaviorTreeSerializer.DeserializeDesc(bytes);
                tree.Title = treeDesc.Title;
                tree.Comment = treeDesc.Comment;
                tree.RootTaskId = treeDesc.RootTaskId;

                if (treeDesc.Tasks != null && treeDesc.Tasks.Length > 0)
                {
                    tree.NextTaskId = treeDesc.Tasks.Max(t => t.Id) + 1;

                    foreach (var taskDesc in treeDesc.Tasks)
                    {
                        var taskId = TaskState.GetId(taskDesc.Id);
                        var task = new TaskState(domain, taskId);
                        if (taskDesc.Id != treeDesc.RootTaskId)
                            task.ParentTaskId = EditorHelper.FindParentTask(treeDesc.Tasks, taskDesc.Id).Id;
                        task.Desc = taskDesc;
                        repo.States[task.Id] = task;
                    }
                }
            }

            Domain = domain;
            Tree = tree;
        }

        private void Domain_OnEventApplied(EditorState state, EditorEvent e)
        {
            try
            {
                var desc = Tree.BuildDesc();

                bytes = BehaviorTreeSerializer.SerializeDesc(desc);

                EditorUtility.SetDirty(this);

                Logger.Debug("Save asset of '{0}'", desc.Title);
            }
            catch (Exception ex)
            {
                Logger.Debug("Failed to save asset due to error: {0}", ex.Message);
            }
        }
    }
}
