using System;

namespace BehaveAsSakura.Editor
{
    public class StringPropertyItem : PropertyItem
    {
        public StringPropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value)
            : base(domain, parent, string.Format("{0}-{1}", typeof(StringPropertyItem), Guid.NewGuid()), name, valueType, value)
        { }

        public override void OnGUI()
        {
            base.OnGUI();

            var newValue = EditorHelper.TextField(Name, (string)Value, LabelClick);
            if (newValue != (string)Value)
            {
                Value = newValue;
                IsDirty = true;
            }
        }
    }
}
