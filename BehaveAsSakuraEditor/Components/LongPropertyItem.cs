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
            var value = (int)Convert.ChangeType(Value, typeof(long));
            var newValue = EditorHelper.LongField(Name, (long)Value, LabelClick);
            if (newValue != value)
            {
                Value = Convert.ChangeType(newValue, ValueType);
                IsDirty = true;
            }
        }
    }
}
