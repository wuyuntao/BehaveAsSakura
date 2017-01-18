using System;

namespace BehaveAsSakura.Editor
{
    class CreateTaskCommand : EditorCommand
    {
        public Type TaskType { get; set; }

        public CreateTaskCommand(string id)
            : base(id)
        {
        }
    }
}
