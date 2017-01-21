using BehaveAsSakura.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BehaveAsSakura.Editor
{
    public class PropertyGroup : EditorComponent
    {
        private Type valueType;
        private object value;

        public PropertyGroup(EditorDomain domain, EditorComponent parent, Type valueType, object value)
            : base(domain
                  , parent
                  , string.Format("{0}-{1}", typeof(PropertyGroup).Name, Guid.NewGuid().ToString()))
        {
            this.valueType = valueType;
            this.value = value;

            Children.AddRange(from p in this.valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              where p.GetCustomAttributes(typeof(BehaveAsFieldAttribute), true).Length > 0
                              select CreateItem(EditorHelper.GetPropertyName(p), p.PropertyType, p.GetValue(value, null)) into i
                              where i != null
                              select (EditorComponent)i);
        }

        public PropertyItem CreateItem(string name, Type valueType, object value)
        {
            if (valueType == typeof(string))
                return new StringPropertyItem(Domain, this, name, valueType, value);

            if (valueType == typeof(bool))
                return new BooleanPropertyItem(Domain, this, name, valueType, value);

            if (valueType == typeof(byte) || valueType == typeof(sbyte)
                    || valueType == typeof(short) || valueType == typeof(ushort)
                    || valueType == typeof(int) || valueType == typeof(uint)
                    || valueType == typeof(long) || valueType == typeof(ulong))
                return new IntPropertyItem(Domain, this, name, valueType, value);

            if (valueType == typeof(long) || valueType == typeof(ulong))
                return new LongPropertyItem(Domain, this, name, valueType, value);

            if (valueType == typeof(float))
                return new FloatPropertyItem(Domain, this, name, valueType, value);

            if (valueType == typeof(double))
                return new DoublePropertyItem(Domain, this, name, valueType, value);

            if (valueType.IsEnum)
                return new EnumPropertyItem(Domain, this, name, valueType, value);

            if (valueType.GetCustomAttributes(typeof(BehaveAsTableAttribute), true).Length > 0)
                return new GroupPropertyItem(Domain, this, name, valueType, value);

            if (valueType.IsArray)
                return CreateListItem(name, valueType, value, valueType.GetElementType());

            if ((typeof(List<>)).IsAssignableFrom(valueType))
                return CreateListItem(name, valueType, value, valueType.GetGenericArguments()[0]);

            Logger.Error("Unsupported property. Name: {0}, Type: {1}", name, valueType);
            return null;
        }

        private PropertyItem CreateListItem(string name, Type valueType, object value, Type elementType)
        {
            if (elementType == typeof(string))
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            if (elementType == typeof(bool))
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            if (elementType == typeof(byte) || elementType == typeof(sbyte)
                    || elementType == typeof(short) || elementType == typeof(ushort)
                    || elementType == typeof(int) || elementType == typeof(uint)
                    || elementType == typeof(long) || elementType == typeof(ulong))
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            if (elementType == typeof(long) || elementType == typeof(ulong))
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            if (elementType == typeof(float))
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            if (elementType == typeof(double))
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            if (elementType.IsEnum)
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            if (elementType.GetCustomAttributes(typeof(BehaveAsTableAttribute), true).Length > 0)
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            Logger.Error("Unsupported list property. Name: {0}, Type: {1}", name, elementType);
            return null;
        }
    }
}
