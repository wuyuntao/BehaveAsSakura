using BehaveAsSakura.Tasks;
using System.Linq;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    static class LayoutHelper
    {
        public static void Calculate(BehaviorTreeState tree)
        {
            if (tree.RootTaskId == 0)
                return;

            var root = CreateTaskLayoutInfos(tree, tree.RootTaskId);

            root.Task.Position = new Vector2(EditorConfiguration.BehaviorTreeNodePosition.x, 
                EditorConfiguration.BehaviorTreeNodePosition.y + EditorConfiguration.TaskNodeMinSpace.y + EditorConfiguration.BehaviorTreeNodeSize.y / 2 + EditorConfiguration.TaskNodeSize.y / 2);

            CalculateTaskPosition(root);
        }

        private static Node CreateTaskLayoutInfos(BehaviorTreeState tree, uint taskId)
        {
            var task = (TaskState)tree.Repository.States[TaskState.GetId(taskId)];
            var node = new Node() { Task = task };

            node.LaneWidth = EditorConfiguration.TaskNodeSize.x + EditorConfiguration.TaskNodeMinSpace.x / 2;

            if (!node.Task.IsCollapsed)
            {
                if (node.Task.Desc is DecoratorTaskDescWrapper)
                {
                    var childTaskId = ((DecoratorTaskDescWrapper)node.Task.Desc).ChildTaskId;
                    if (childTaskId > 0)
                    {
                        var childTaskInfo = CreateTaskLayoutInfos(tree, childTaskId);

                        node.Children = new[] { childTaskInfo };
                        node.LaneWidth = childTaskInfo.LaneWidth;
                    }
                }
                else if (node.Task.Desc is CompositeTaskDescWrapper)
                {
                    var childTaskIds = ((CompositeTaskDescWrapper)node.Task.Desc).ChildTaskIds;
                    if (childTaskIds.Count > 0)
                    {
                        node.Children = (childTaskIds.Select(childTaskId => CreateTaskLayoutInfos(tree, childTaskId))).ToArray();
                        node.LaneWidth = node.Children.Sum(i => i.LaneWidth);
                    }
                }
            }

            return node;
        }

        private static void CalculateTaskPosition(Node node)
        {
            if (node.Children == null)
                return;

            var x = node.Task.Position.x - node.Children.Sum(i => i.LaneWidth) / 2;
            var y = node.Task.Position.y + EditorConfiguration.TaskNodeMinSpace.y + EditorConfiguration.TaskNodeSize.y;

            foreach (var child in node.Children)
            {
                child.Task.Position = new Vector2(x + child.LaneWidth / 2, y);

                CalculateTaskPosition(child);

                x += child.LaneWidth;
            }
        }

        class Node
        {
            public TaskState Task { get; set; }

            public Node[] Children { get; set; }

            public float LaneWidth { get; set; }
        }

    }
}
