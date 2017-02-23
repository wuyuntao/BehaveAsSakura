using System;

namespace BehaveAsSakura.Editor
{
    public class MoveTaskCommand : EditorCommand
    {
        public bool Left { get; set; }

        public MoveTaskCommand(string id)
            : base(id)
        {
        }
    }
}
