using System;

namespace BehaveAsSakura.Editor
{
    public class MoveTaskCommand : EditorCommand
    {
        public int Offset { get; set; }

        public MoveTaskCommand(string id)
            : base(id)
        {
        }
    }
}
