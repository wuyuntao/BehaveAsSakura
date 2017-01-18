using BehaveAsSakura.Tasks;
using System;

namespace BehaveAsSakura.Editor
{
    public class BehaviorTreeCommandHandler : EditorCommandHandler
    {
        public override void ProcessCommand(EditorCommand command)
        {
            if (command is CreateTaskCommand)
            {
                OnCreateTaskCommand((CreateTaskCommand)command);
            }
            else if (command is RemoveTaskCommand)
            {
                OnRemoveTaskCommand((RemoveTaskCommand)command);
            }
        }

        private void OnCreateTaskCommand(CreateTaskCommand command)
        {
            var parent = Repository.States[command.Id];
            if (parent is BehaviorTreeState)
            {
                var tree = (BehaviorTreeState)parent;
                if (tree.RootTask != null)
                {
                    parent.ApplyEvent(new TaskNotCreatedEvent(command.Id)
                    {
                        Reason = "Behavior tree already has root task"
                    });
                    return;
                }
            }
            else if (parent is TaskState)
            {
                var task = (TaskState)parent;
                if (task.Desc is LeafTaskDescWrapper)
                {
                    parent.ApplyEvent(new TaskNotCreatedEvent(command.Id)
                    {
                        Reason = "Leaf task cannot have child task"
                    });
                    return;
                }
                else if (task.Desc is DecoratorTaskDescWrapper)
                {
                    var desc = (DecoratorTaskDescWrapper)task.Desc;
                    if (desc.ChildTaskId != 0)
                    {
                        parent.ApplyEvent(new TaskNotCreatedEvent(command.Id)
                        {
                            Reason = "Decorator task can only has one child task"
                        });
                        return;
                    }
                }
            }
            else
                throw new NotSupportedException(command.Id);

            parent.ApplyEvent(new TaskCreatedEvent(command.Id)
            {
                TaskType = command.TaskType
            });
        }

        private void OnRemoveTaskCommand(RemoveTaskCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
