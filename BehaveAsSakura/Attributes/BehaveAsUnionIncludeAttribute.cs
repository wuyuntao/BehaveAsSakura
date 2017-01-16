using System;

namespace BehaveAsSakura.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BehaveAsUnionIncludeAttribute : Attribute
    {
        public BehaveAsUnionIncludeAttribute(Type unionType, int tag)
        {
            Tag = tag;
            UnionType = unionType;
        }

        public Type UnionType { get; private set; }

        public int Tag { get; private set; }
    }
}
