using System;

namespace BehaveAsSakura.Editor
{
    class EnumPropertyItem : PropertyItem
    {
        public EnumPropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value)
            : base(domain, parent, string.Format("{0}-{1}", typeof(EnumPropertyItem), Guid.NewGuid()), name, valueType, value)
        { }

        public override void OnGUI()
        {
            base.OnGUI();

            var newValue = EditorHelper.EnumPopup(Name, (Enum)Value, LabelClick);
            if (newValue != (Enum)Value)
            {
                Value = newValue;
                IsDirty = true;
            }
        }
    }
}
