using System;

namespace BehaveAsSakura.Editor
{
    public class CreateTaskCommand : EditorCommand
    {
        public Type TaskType { get; set; }

        public CreateTaskCommand(string id)
            : base(id)
        {
        }
    }
}
