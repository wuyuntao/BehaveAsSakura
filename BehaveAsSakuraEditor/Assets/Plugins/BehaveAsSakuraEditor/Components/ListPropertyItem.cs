using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    class ListPropertyItem : PropertyItem
    {
        private Type elementType;

        private PropertyItem[] elements;
        private bool showList = true;

        public ListPropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value, Type elementType)
            : base(domain, parent, string.Format("{0}-{1}", typeof(ListPropertyItem), Guid.NewGuid()), name, valueType, value)
        {
            this.elementType = elementType;

            if (value == null)
                elements = new PropertyItem[0];
            else
            {
                var list = (IList)value;

                elements = new PropertyItem[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    elements[i] = parent.CreateItem(I18n._("Element #{0}", i), elementType, list[i]);
                    elements[i].OnLabelClick += Item_OnLabelClick;
                }
            }
        }

        public override void OnGUI()
        {
            EditorHelper.Foldout(ref showList, Name, () =>
            {
                var size = Math.Max(0, EditorHelper.IntField(I18n._("Size"), elements.Length));

                if (size != elements.Length)
                {
                    var newElements = new PropertyItem[size];
                    Array.Copy(elements, newElements, Math.Min(size, elements.Length));

                    if (newElements.Length > elements.Length)
                    {
                        for (var i = elements.Length; i < newElements.Length; i++)
                        {
                            newElements[i] = ((PropertyGroup)Parent).CreateItem(I18n._("Element #{0}", i), elementType, Activator.CreateInstance(elementType));
                            newElements[i].OnLabelClick += Item_OnLabelClick;
                        }
                    }

                    elements = newElements;
                }

                for (int i = 0; i < elements.Length; i++)
                    elements[i].OnGUI();
            }, LabelClick);
        }

        private void Item_OnLabelClick(PropertyItem item, Event e)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent(I18n._("Delete element")), false, () => Item_OnDeleteElement(item));

            menu.ShowAsContext();
        }

        private void Item_OnDeleteElement(PropertyItem item)
        {
            var newElements = new PropertyItem[elements.Length - 1];

            var i = Array.IndexOf(elements, item);
            if (i > 0)
                Array.Copy(elements, newElements, i);

            Array.Copy(elements, i + 1, newElements, i, elements.Length - i - 1);

            for (var j = i; j < newElements.Length; j++)
                newElements[j].Name = I18n._("Element #{0}", j);

            elements = newElements;
        }
    }
}
