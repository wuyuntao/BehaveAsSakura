using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class EditorObject
    {
        public string Id { get; private set; }

        protected EditorObject(string id)
        {
            Id = id;
        }
    }
}
