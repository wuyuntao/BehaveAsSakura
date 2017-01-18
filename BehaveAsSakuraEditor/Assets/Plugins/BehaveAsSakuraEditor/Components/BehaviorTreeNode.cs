using System;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeNode : Node
    {
        public BehaviorTreeNode(EditorDomain domain, BehaviorTreeView parent, string title)
            : base(domain, parent, title, EditorConfiguration.BehaviorTreeNodePosition, EditorConfiguration.BehaviorTreeNodeSize, EditorConfiguration.BehaviorTreeNodeStyle)
        {
        }

        public override void OnContextMenu(Event e)
        {
            base.OnContextMenu(e);

            var menu = new GenericMenu();
            var tree = (BehaviorTreeState)Repository.States[BehaviorTreeState.DefaultId];
            EditorHelper.AddNewTaskMenuItems(menu, tree.RootTaskId == 0, OnContextMenu_NewTask);
            menu.ShowAsContext();

            e.Use();
        }

        private void OnContextMenu_NewTask(object userData)
        {
            var taskType = (Type)userData;

            Debug.LogFormat("OnContextMenu_NewTask {0}", taskType);
        }
    }
}
