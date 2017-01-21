using BehaveAsSakura.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BehaveAsSakura.Editor
{
    public class PropertyGroup : EditorComponent
    {
        public Type ValueType { get; private set; }

        public object Value { get; private set; }

        public bool IsDirty { get; private set; }

        private Tuple<PropertyItem, PropertyInfo>[] properties;

        public PropertyGroup(EditorDomain domain, EditorComponent parent, Type valueType, object value)
            : base(domain
                  , parent
                  , string.Format("{0}-{1}", typeof(PropertyGroup).Name, Guid.NewGuid().ToString()))
        {
            ValueType = valueType;
            Value = value;

            properties = (from p in ValueType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                          where p.GetCustomAttributes(typeof(BehaveAsFieldAttribute), true).Length > 0
                          select Tuple.Create(CreateItem(EditorHelper.GetPropertyName(p), p.PropertyType, p.GetValue(value, null)), p) into t
                          where t.Item2 != null
                          select t).ToArray();
        }

        public PropertyItem CreateItem(string name, Type valueType, object value)
        {
            if (valueType == typeof(string))
                return new StringPropertyItem(Domain, this, name, valueType, value);

            if (valueType == typeof(bool))
                return new BooleanPropertyItem(Domain, this, name, valueType, value);

            if (valueType == typeof(byte) || valueType == typeof(sbyte)
                    || valueType == typeof(short) || valueType == typeof(ushort)
                    || valueType == typeof(int) || valueType == typeof(uint))
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
            if (elementType == typeof(string)
                    || elementType == typeof(bool)
                    || elementType == typeof(byte) || elementType == typeof(sbyte)
                    || elementType == typeof(short) || elementType == typeof(ushort)
                    || elementType == typeof(int) || elementType == typeof(uint)
                    || elementType == typeof(long) || elementType == typeof(ulong)
                    || elementType == typeof(float) || elementType == typeof(double)
                    || elementType.IsEnum
                    || elementType.GetCustomAttributes(typeof(BehaveAsTableAttribute), true).Length > 0)
                return new ListPropertyItem(Domain, this, name, valueType, value, elementType);

            Logger.Error("Unsupported list property. Name: {0}, Type: {1}", name, elementType);
            return null;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            IsDirty = false;
            foreach (var p in properties)
            {
                p.Item1.OnGUI();

                if (p.Item1.IsDirty)
                {
                    p.Item2.SetValue(Value, p.Item1.Value, null);
                    IsDirty |= true;
                }
            }
        }
    }
}
