using BehaveAsSakura.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

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
                                select CreateItem(EditorHelper.GetPropertyName(p), p.PropertyType, p.GetValue(value, null)) into i
                                where i != null
                                select i);
        }

        public void OnGUI()
        {
            foreach (var p in properties)
                p.OnGUI();
        }

        private Item CreateItem(string name, Type valueType, object value)
        {
            if (valueType == typeof(string))
                return new StringItem(this, name, valueType, value);

            if (valueType == typeof(bool))
                return new BooleanItem(this, name, valueType, value);

            if (valueType == typeof(byte) || valueType == typeof(sbyte)
                    || valueType == typeof(short) || valueType == typeof(ushort)
                    || valueType == typeof(int) || valueType == typeof(uint)
                    || valueType == typeof(long) || valueType == typeof(ulong))
                return new IntItem(this, name, valueType, value);

            if (valueType == typeof(long) || valueType == typeof(ulong))
                return new LongItem(this, name, valueType, value);

            if (valueType == typeof(float))
                return new FloatItem(this, name, valueType, value);

            if (valueType == typeof(double))
                return new DoubleItem(this, name, valueType, value);

            if (valueType.IsEnum)
                return new EnumItem(this, name, valueType, value);

            if (valueType.GetCustomAttributes(typeof(BehaveAsTableAttribute), true).Length > 0)
                return new PropertyGroupItem(this, name, valueType, value);

            if (valueType.IsArray)
                return CreateListItem(name, valueType, value, valueType.GetElementType());

            if ((typeof(List<>)).IsAssignableFrom(valueType))
                return CreateListItem(name, valueType, value, valueType.GetGenericArguments()[0]);

            Logger.Error("Unsupported property. Name: {0}, Type: {1}", name, valueType);
            return null;
        }

        private Item CreateListItem(string name, Type valueType, object value, Type elementType)
        {
            if (elementType == typeof(string))
                return new ListItem(this, name, valueType, value, elementType);

            if (elementType == typeof(bool))
                return new ListItem(this, name, valueType, value, elementType);

            if (elementType == typeof(byte) || elementType == typeof(sbyte)
                    || elementType == typeof(short) || elementType == typeof(ushort)
                    || elementType == typeof(int) || elementType == typeof(uint)
                    || elementType == typeof(long) || elementType == typeof(ulong))
                return new ListItem(this, name, valueType, value, elementType);

            if (elementType == typeof(long) || elementType == typeof(ulong))
                return new ListItem(this, name, valueType, value, elementType);

            if (elementType == typeof(float))
                return new ListItem(this, name, valueType, value, elementType);

            if (elementType == typeof(double))
                return new ListItem(this, name, valueType, value, elementType);

            if (elementType.IsEnum)
                return new ListItem(this, name, valueType, value, elementType);

            if (elementType.GetCustomAttributes(typeof(BehaveAsTableAttribute), true).Length > 0)
                return new ListItem(this, name, valueType, value, elementType);

            Logger.Error("Unsupported list property. Name: {0}, Type: {1}", name, elementType);
            return null;
        }

        abstract class Item
        {
            public event Action<Item, Event> OnLabelClick;

            public PropertyGroup owner;
            public string name;
            public Type valueType;
            public object value;

            protected Item(PropertyGroup owner, string name, Type valueType, object value)
            {
                this.owner = owner;
                this.name = name;
                this.valueType = valueType;
                this.value = value;
            }

            public abstract void OnGUI();

            protected virtual void LabelClick(Event e)
            {
                Logger.Debug("LabelClick {0}", name);

                if (OnLabelClick != null)
                    OnLabelClick(this, e);
            }
        }

        class StringItem : Item
        {
            public StringItem(PropertyGroup owner, string name, Type valueType, object value)
                : base(owner, name, valueType, value)
            { }

            public override void OnGUI()
            {
                value = EditorHelper.TextField(name, (string)value, LabelClick);
            }
        }

        class BooleanItem : Item
        {
            public BooleanItem(PropertyGroup owner, string name, Type valueType, object value)
                : base(owner, name, valueType, value)
            { }

            public override void OnGUI()
            {
                value = EditorHelper.Toggle(name, (bool)value, LabelClick);
            }
        }

        class IntItem : Item
        {
            public IntItem(PropertyGroup owner, string name, Type valueType, object value)
                : base(owner, name, valueType, value)
            { }

            public override void OnGUI()
            {
                // TODO limit range for integer fields
                value = EditorHelper.IntField(name, (int)value, LabelClick);
            }
        }

        class LongItem : Item
        {
            public LongItem(PropertyGroup owner, string name, Type valueType, object value)
                : base(owner, name, valueType, value)
            { }

            public override void OnGUI()
            {
                // TODO limit range for integer fields
                value = EditorHelper.LongField(name, (long)value, LabelClick);
            }
        }

        class FloatItem : Item
        {
            public FloatItem(PropertyGroup owner, string name, Type valueType, object value)
                : base(owner, name, valueType, value)
            { }

            public override void OnGUI()
            {
                // TODO limit range for integer fields
                value = EditorHelper.FloatField(name, (float)value, LabelClick);
            }
        }

        class DoubleItem : Item
        {
            public DoubleItem(PropertyGroup owner, string name, Type valueType, object value)
                : base(owner, name, valueType, value)
            { }

            public override void OnGUI()
            {
                // TODO limit range for integer fields
                value = EditorHelper.DoubleField(name, (double)value, LabelClick);
            }
        }

        class EnumItem : Item
        {
            public EnumItem(PropertyGroup owner, string name, Type valueType, object value)
                : base(owner, name, valueType, value)
            { }

            public override void OnGUI()
            {
                value = EditorHelper.EnumPopup(name, (Enum)value, LabelClick);
            }
        }

        class PropertyGroupItem : Item
        {
            private PropertyGroup group;
            private bool showGroup = true;

            public PropertyGroupItem(PropertyGroup owner, string name, Type valueType, object value)
                : base(owner, name, valueType, value)
            {
                if (value == null)
                    value = Activator.CreateInstance(valueType);

                group = new PropertyGroup(valueType, value);
            }

            public override void OnGUI()
            {
                EditorHelper.Foldout(ref showGroup, name, () => group.OnGUI(), LabelClick);
            }
        }

        class ListItem : Item
        {
            private Type elementType;

            private Item[] elements;
            private bool showList = true;

            public ListItem(PropertyGroup owner, string name, Type valueType, object value, Type elementType)
                : base(owner, name, valueType, value)
            {
                this.elementType = elementType;

                if (value == null)
                    elements = new Item[0];
                else
                {
                    var list = (IList)value;

                    elements = new Item[list.Count];
                    for (int i = 0; i < list.Count; i++)
                    {
                        elements[i] = owner.CreateItem(I18n._("Element #{0}", i), elementType, list[i]);
                        elements[i].OnLabelClick += Item_OnLabelClick;
                    }
                }
            }

            public override void OnGUI()
            {
                EditorHelper.Foldout(ref showList, name, () =>
                {
                    var size = Math.Max(0, EditorHelper.IntField(I18n._("Size"), elements.Length));

                    if (size != elements.Length)
                    {
                        var newElements = new Item[size];
                        Array.Copy(elements, newElements, Math.Min(size, elements.Length));

                        if (newElements.Length > elements.Length)
                        {
                            for (var i = elements.Length; i < newElements.Length; i++)
                            {
                                newElements[i] = owner.CreateItem(I18n._("Element #{0}", i), elementType, Activator.CreateInstance(elementType));
                                newElements[i].OnLabelClick += Item_OnLabelClick;
                            }
                        }

                        elements = newElements;
                    }

                    for (int i = 0; i < elements.Length; i++)
                        elements[i].OnGUI();
                }, LabelClick);
            }

            private void Item_OnLabelClick(Item item, Event e)
            {
                var menu = new GenericMenu();

                menu.AddItem(new GUIContent(I18n._("Delete element")), false, () => Item_OnDeleteElement(item));

                menu.ShowAsContext();
            }

            private void Item_OnDeleteElement(Item item)
            {
                var newElements = new Item[elements.Length - 1];

                var i = Array.IndexOf(elements, item);
                if (i > 0)
                    Array.Copy(elements, newElements, i);

                Array.Copy(elements, i + 1, newElements, i, elements.Length - i - 1);

                for (var j = i; j < newElements.Length; j++)
                    newElements[j].name = I18n._("Element #{0}", j);

                elements = newElements;
            }
        }
    }
}
