using System;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class PropertyItem : EditorComponent
    {
        public event Action<PropertyItem, Event> OnLabelClick;

        public string Name { get; set; }

        public Type ValueType { get; protected set; }

        public object Value { get; protected set; }

        protected PropertyItem(EditorDomain domain, PropertyGroup parent, string id, string name, Type valueType, object value)
            : base(domain, parent, id)
        {
            Name = name;
            ValueType = valueType;
            Value = value;
        }

        protected virtual void LabelClick(Event e)
        {
            //Logger.Debug("LabelClick {0}", Name);

            if (OnLabelClick != null)
                OnLabelClick(this, e);
        }
    }
}
