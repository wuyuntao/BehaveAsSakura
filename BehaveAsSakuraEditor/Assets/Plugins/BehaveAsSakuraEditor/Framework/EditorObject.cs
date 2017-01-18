﻿using System;

namespace BehaveAsSakura.Editor
{
    [Serializable]
    public abstract class EditorObject
    {
        public string Id { get; private set; }

        protected EditorObject(string id)
        {
            Id = id;
        }
    }
}