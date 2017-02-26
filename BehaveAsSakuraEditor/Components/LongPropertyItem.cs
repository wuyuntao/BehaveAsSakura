using System;

namespace BehaveAsSakura.Editor
{
    class LongPropertyItem : PropertyItem
    {
        public LongPropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value)
            : base(domain, parent, string.Format("{0}-{1}", typeof(LongPropertyItem), Guid.NewGuid()), name, valueType, value)
        { }

        public override void OnGUI()
        {
            base.OnGUI();

            // TODO limit range for integer fields
            var newValue = EditorHelper.LongField(Name, (long)Value, LabelClick);
            if (newValue != (long)Value)
            {
                Value = newValue;
                IsDirty = true;
            }
        }
    }
}
