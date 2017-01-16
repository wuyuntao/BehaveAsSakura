using System;

namespace BehaveAsSakura.SerializationCompiler.Schema
{
    class FieldDef
    {
        public string Name { get; private set; }

        public int Tag { get; private set; }

        public Type Type { get; private set; }

        public FieldDef(string name, int tag, Type type)
        {
            Name = name;
            Tag = tag;
            Type = type;
        }
    }
}
