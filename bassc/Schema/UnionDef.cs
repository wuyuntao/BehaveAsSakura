using System;
using System.Collections.Generic;

namespace BehaveAsSakura.SerializationCompiler.Schema
{
    class UnionDef
    {
        public Type UnionType { get; private set; }

        public List<Tuple<Type, int>> IncludedTypes { get; private set; }

        public UnionDef(Type unionType)
        {
            UnionType = unionType;
            IncludedTypes = new List<Tuple<Type, int>>();
        }

        public void Sort()
        {
            IncludedTypes.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
        }
    }
}
