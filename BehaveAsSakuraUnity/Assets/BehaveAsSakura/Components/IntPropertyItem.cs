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
            var newValue = EditorHelper.IntField(Name, (int)Value, LabelClick);
            if (newValue != (int)Value)
            {
                Value = newValue;
                IsDirty = true;
            }
        }
    }
}
