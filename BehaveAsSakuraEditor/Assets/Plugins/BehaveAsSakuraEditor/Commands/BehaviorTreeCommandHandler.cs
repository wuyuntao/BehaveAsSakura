using BehaveAsSakura.Tasks;
using System;
using UnityEditor;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeCommandHandler : EditorCommandHandler
    {
        public override void ProcessCommand(EditorCommand command)
        {
            base.ProcessCommand(command);

            if (command is CreateTaskCommand)
            {
                OnCreateTaskCommand((CreateTaskCommand)command);
            }
            else if (command is RemoveTaskCommand)
            {
                OnRemoveTaskCommand((RemoveTaskCommand)command);
            }
            else if (command is ChangeTaskSummaryCommand)
            {
                OnChangeTaskSummaryCommand((ChangeTaskSummaryCommand)command);
            }
            else if (command is ChangeTaskDescCommand)
            {
                OnChangeTaskPropertyCommand((ChangeTaskDescCommand)command);
            }
            else if (command is MoveTaskCommand)
            {
                OnMoveTaskCommand((MoveTaskCommand)command);
            }
            else if (command is ChangeBehaviorTreeSummaryCommand)
            {
                OnChangeBehaviorTreeSummaryCommand((ChangeBehaviorTreeSummaryCommand)command);
            }
        }

        private void OnCreateTaskCommand(CreateTaskCommand command)
        {
            var parent = Repository.States[command.Id];
            var parentTaskId = 0u;
            if (parent is BehaviorTreeState)
            {
                var tree = (BehaviorTreeState)parent;
                if (tree.RootTaskId > 0)
                {
                    parent.ApplyEvent(new TaskNotCreatedEvent(command.Id) { Reason = "Behavior tree already has root task" });
                    return;
                }
            }
            else if (parent is TaskState)
            {
                var task = (TaskState)parent;
                if (task.Desc is LeafTaskDescWrapper)
                {
                    parent.ApplyEvent(new TaskNotCreatedEvent(command.Id) { Reason = "Leaf task cannot have child task" });
                    return;
                }
                else if (task.Desc is DecoratorTaskDescWrapper)
                {
                    var desc = (DecoratorTaskDescWrapper)task.Desc;
                    if (desc.ChildTaskId != 0)
                    {
                        parent.ApplyEvent(new TaskNotCreatedEvent(command.Id) { Reason = "Decorator task can only has one child task" });
                        return;
                    }
                }

                parentTaskId = task.Desc.Id;
            }
            else
                throw new NotSupportedException(command.Id);

            {
                var tree = (BehaviorTreeState)Repository.States[BehaviorTreeState.GetId()];
                TaskDescWrapper taskDescWrapper;
                if (typeof(ILeafTaskDesc).IsAssignableFrom(command.TaskType))
                    taskDescWrapper = new LeafTaskDescWrapper();
                else if (typeof(IDecoratorTaskDesc).IsAssignableFrom(command.TaskType))
                    taskDescWrapper = new DecoratorTaskDescWrapper();
                else if (typeof(ICompositeTaskDesc).IsAssignableFrom(command.TaskType))
                    taskDescWrapper = new CompositeTaskDescWrapper();
                else
                    throw new NotSupportedException(command.TaskType.ToString());
                taskDescWrapper.Id = tree.NextTaskId;
                taskDescWrapper.CustomDesc = (ITaskDesc)Activator.CreateInstance(command.TaskType);
                var taskState = new TaskState(Domain, TaskState.GetId(tree.NextTaskId));
                taskState.ParentTaskId = parentTaskId;
                taskState.Desc = taskDescWrapper;

                parent.ApplyEvent(new TaskCreatedEvent(command.Id) { NewTask = taskState });
            }
        }

        private void OnRemoveTaskCommand(RemoveTaskCommand command)
        {
            var task = (TaskState)Repository.States[command.Id];

            task.ApplyEvent(new TaskRemovedEvent(command.Id));
        }

        private void OnChangeTaskSummaryCommand(ChangeTaskSummaryCommand command)
        {
            var task = (TaskState)Repository.States[command.Id];

            task.ApplyEvent(new TaskSummaryChangedEvent(command.Id)
            {
                Title = command.Title,
                Comment = command.Comment,
            });
        }

        private void OnChangeTaskPropertyCommand(ChangeTaskDescCommand command)
        {
            var task = (TaskState)Repository.States[command.Id];

            task.ApplyEvent(new TaskPropertyDescEvent(command.Id)
            {
                CustomDesc = command.CustomDesc,
            });
        }

        private void OnMoveTaskCommand(MoveTaskCommand command)
        {
            var task = (TaskState)Repository.States[command.Id];
            if (command.Offset == 0)
            {
                task.ApplyEvent(new TaskNotMovedEvent(command.Id) { Reason = "Zero offset" });
                return;
            }

            if (task.ParentTask == null)
            {
                task.ApplyEvent(new TaskNotMovedEvent(command.Id) { Reason = "Cannot move task" });
                return;
            }

            if (task.ParentTask.Desc is LeafTaskDescWrapper || task.ParentTask.Desc is DecoratorTaskDescWrapper)
            {
                task.ApplyEvent(new TaskNotMovedEvent(command.Id) { Reason = "Cannot move task" });
                return;
            }

            var parentTask = (CompositeTaskDescWrapper)task.ParentTask.Desc;
            if (parentTask.ChildTaskIds.Count == 1)
            {
                task.ApplyEvent(new TaskNotMovedEvent(command.Id) { Reason = "Cannot move task" });
                return;
            }

            var taskIndex = parentTask.ChildTaskIds.IndexOf(task.Desc.Id);
            if (taskIndex + command.Offset < 0)
            {
                task.ApplyEvent(new TaskNotMovedEvent(command.Id) { Reason = "Cannot move task to left" });
                return;
            }

            if (taskIndex + command.Offset >= parentTask.ChildTaskIds.Count)
            {
                task.ApplyEvent(new TaskNotMovedEvent(command.Id) { Reason = "Cannot move task to right" });
                return;
            }

            task.ApplyEvent(new TaskMovedEvent(command.Id) { Offset = command.Offset });
        }

        private void OnChangeBehaviorTreeSummaryCommand(ChangeBehaviorTreeSummaryCommand command)
        {
            var tree = (BehaviorTreeState)Repository.States[command.Id];

            tree.ApplyEvent(new BehaviorTreeSummaryChangedEvent(command.Id)
            {
                Title = command.Title,
                Comment = command.Comment,
            });
        }
    }
}
