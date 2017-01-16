using System;
using System.Collections.Generic;

namespace BehaveAsSakura.SerializationCompiler
{
    class TaskDef
    {
        public TaskDef(string name, IEnumerable<PropertyDef> desc, IEnumerable<PropertyDef> props)
        {
            Name = name;
            Desc = desc;
            Props = props;
        }

        public string Name { get; private set; }

        public IEnumerable<PropertyDef> Desc { get; private set; }

        public IEnumerable<PropertyDef> Props { get; private set; }
    }

    class PropertyDef
    {
        public PropertyDef(string name, int tag, Type type)
        {
            Name = name;
            Tag = tag;
            Type = type;
        }

        public string Name { get; private set; }

        public int Tag { get; private set; }

        public Type Type { get; private set; }
    }
}
