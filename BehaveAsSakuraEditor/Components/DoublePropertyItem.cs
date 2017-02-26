using System;

namespace BehaveAsSakura.Editor
{
    class DoublePropertyItem : PropertyItem
    {
        public DoublePropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value)
            : base(domain, parent, string.Format("{0}-{1}", typeof(DoublePropertyItem), Guid.NewGuid()), name, valueType, value)
        { }

        public override void OnGUI()
        {
            base.OnGUI();

            // TODO limit range for integer fields
            var newValue = EditorHelper.DoubleField(Name, (double)Value, LabelClick);
            if (newValue != (double)Value)
            {
                Value = newValue;
                IsDirty = true;
            }
        }
    }
}
