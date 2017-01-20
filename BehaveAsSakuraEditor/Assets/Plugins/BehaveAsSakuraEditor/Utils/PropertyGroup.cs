using BehaveAsSakura.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace BehaveAsSakura.Editor
{
    public class PropertyGroup
    {
        private Type valueType;
        private object value;
        private List<Item> properties = new List<Item>();

        public PropertyGroup(Type valueType, object value)
        {
            this.valueType = valueType;
            this.value = value;

            properties.AddRange(from p in this.valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                where p.GetCustomAttributes(typeof(BehaveAsFieldAttribute), true).Length > 0
                                select CreateItem(p) into i
                                where i != null
                                select i);
        }

        public void OnGUI()
        {
            foreach (var p in properties)
                p.OnGUI();
        }

        private Item CreateItem(PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(string))
                return new StringItem(this, pi);

            if (pi.PropertyType == typeof(bool))
                return new BooleanItem(this, pi);

            if (pi.PropertyType == typeof(byte) || pi.PropertyType == typeof(sbyte)
                    || pi.PropertyType == typeof(short) || pi.PropertyType == typeof(ushort)
                    || pi.PropertyType == typeof(int) || pi.PropertyType == typeof(uint)
                    || pi.PropertyType == typeof(long) || pi.PropertyType == typeof(ulong))
                return new IntItem(this, pi);

            if (pi.PropertyType == typeof(long) || pi.PropertyType == typeof(ulong))
                return new LongItem(this, pi);

            if (pi.PropertyType == typeof(float))
                return new FloatItem(this, pi);

            if (pi.PropertyType == typeof(double))
                return new DoubleItem(this, pi);

            if (pi.PropertyType.IsEnum)
                return new EnumItem(this, pi);

            if (pi.PropertyType.GetCustomAttributes(typeof(BehaveAsTableAttribute), true).Length > 0)
                return new PropertyGroupItem(this, pi);

            Logger.Error("Not implemented yet: {0}", pi.PropertyType);
            return null;
        }

        abstract class Item
        {
            protected PropertyGroup owner;
            protected PropertyInfo pi;
            protected string name;
            protected object value;

            protected Item(PropertyGroup owner, PropertyInfo pi)
            {
                this.owner = owner;
                this.pi = pi;

                name = EditorHelper.GetPropertyName(pi);
                value = pi.GetValue(owner.value, null);
            }

            public abstract void OnGUI();
        }

        class StringItem : Item
        {
            public StringItem(PropertyGroup owner, PropertyInfo pi)
                : base(owner, pi)
            { }

            public override void OnGUI()
            {
                value = EditorGUILayout.TextField(name, (string)value);
            }
        }

        class BooleanItem : Item
        {
            public BooleanItem(PropertyGroup owner, PropertyInfo pi)
                : base(owner, pi)
            { }

            public override void OnGUI()
            {
                value = EditorGUILayout.Toggle(name, (bool)value);
            }
        }

        class IntItem : Item
        {
            public IntItem(PropertyGroup owner, PropertyInfo pi)
                : base(owner, pi)
            { }

            public override void OnGUI()
            {
                // TODO limit range for integer fields
                value = EditorGUILayout.IntField(name, (int)value);
            }
        }

        class LongItem : Item
        {
            public LongItem(PropertyGroup owner, PropertyInfo pi)
                : base(owner, pi)
            { }

            public override void OnGUI()
            {
                // TODO limit range for integer fields
                value = EditorGUILayout.LongField(name, (long)value);
            }
        }

        class FloatItem : Item
        {
            public FloatItem(PropertyGroup owner, PropertyInfo pi)
                : base(owner, pi)
            { }

            public override void OnGUI()
            {
                // TODO limit range for integer fields
                value = EditorGUILayout.FloatField(name, (float)value);
            }
        }

        class DoubleItem : Item
        {
            public DoubleItem(PropertyGroup owner, PropertyInfo pi)
                : base(owner, pi)
            { }

            public override void OnGUI()
            {
                // TODO limit range for integer fields
                value = EditorGUILayout.DoubleField(name, (double)value);
            }
        }

        class EnumItem : Item
        {
            public EnumItem(PropertyGroup owner, PropertyInfo pi)
                : base(owner, pi)
            { }

            public override void OnGUI()
            {
                value = EditorGUILayout.EnumPopup(name, (Enum)value);
            }
        }

        class PropertyGroupItem : Item
        {
            private PropertyGroup group;
            private bool showGroup = true;

            public PropertyGroupItem(PropertyGroup owner, PropertyInfo pi)
                : base(owner, pi)
            {
                if (value == null)
                    value = Activator.CreateInstance(pi.PropertyType);

                group = new PropertyGroup(pi.PropertyType, value);
            }

            public override void OnGUI()
            {
                EditorHelper.Foldout(ref showGroup, name, () => group.OnGUI());
            }
        }
    }
}
