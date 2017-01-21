using System;

namespace BehaveAsSakura.Editor
{
    public class BooleanPropertyItem : PropertyItem
    {
        public BooleanPropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value)
            : base(domain, parent, string.Format("{0}-{1}", typeof(BooleanPropertyItem), Guid.NewGuid()), name, valueType, value)
        { }

        public override void OnGUI()
        {
            base.OnGUI();

            var newValue = EditorHelper.Toggle(Name, (bool)Value, LabelClick);
            if (newValue != (bool)Value)
            {
                Value = newValue;
                IsDirty = true;
            }
        }
    }
}
