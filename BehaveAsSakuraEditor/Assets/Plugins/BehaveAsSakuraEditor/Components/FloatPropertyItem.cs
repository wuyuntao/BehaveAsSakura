using System;

namespace BehaveAsSakura.Editor
{
    class FloatPropertyItem : PropertyItem
    {
        public FloatPropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value)
            : base(domain, parent, string.Format("{0}-{1}", typeof(FloatPropertyItem), Guid.NewGuid()), name, valueType, value)
        { }

        public override void OnGUI()
        {
            base.OnGUI();

            // TODO limit range for integer fields
            Value = EditorHelper.FloatField(Name, (float)Value, LabelClick);
        }
    }
}
