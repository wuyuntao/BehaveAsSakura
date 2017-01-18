using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public class CreateTaskCommand : EditorCommand
    {
        public Type TaskType { get; set; }

        public Vector2 TaskPosition { get; set; }

        public CreateTaskCommand(string id)
            : base(id)
        {
        }
    }
}
