using System;

namespace BehaveAsSakura.SerializationCompiler.Schema
{
    class EnumDef
    {
        public Type Type { get; private set; }

        public EnumDef(Type type)
        {
            Type = type;
        }
    }
}
