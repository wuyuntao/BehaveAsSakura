using BehaveAsSakura.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BehaveAsSakura.SerializationCompiler.Schema
{
    class TableDef
    {
        public Type Type { get; private set; }

        public List<FieldDef> Fields { get; private set; }

        public TableDef(Type type)
        {
            Type = type;

            Fields = new List<FieldDef>();

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var attr = property.GetCustomAttribute<BehaveAsFieldAttribute>();
                if (attr != null)
                    Fields.Add(new FieldDef(property.Name, attr.Tag, property.PropertyType));
            }
        }
    }
}
