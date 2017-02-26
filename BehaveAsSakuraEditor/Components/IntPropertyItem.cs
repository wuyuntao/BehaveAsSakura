using System;

namespace BehaveAsSakura.Editor
{
    class IntPropertyItem : PropertyItem
    {
        public IntPropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value)
            : base(domain, parent, string.Format("{0}-{1}", typeof(IntPropertyItem), Guid.NewGuid()), name, valueType, value)
        { }

        public override void OnGUI()
        {
            base.OnGUI();

            // TODO limit range for integer fields
            var value = (int)Convert.ChangeType(Value, typeof(int));
            var newValue = EditorHelper.IntField(Name, value, LabelClick);
            if (newValue != value)
            {
                Value = Convert.ChangeType(newValue, ValueType);
                IsDirty = true;
            }
        }
    }
}
