using System;

namespace BehaveAsSakura.Editor
{
    class GroupPropertyItem : PropertyItem
    {
        private PropertyGroup group;
        private bool showGroup = true;

        public GroupPropertyItem(EditorDomain domain, PropertyGroup parent, string name, Type valueType, object value)
            : base(domain, parent, string.Format("{0}-{1}", typeof(GroupPropertyItem), Guid.NewGuid()), name, valueType, value)
        {
            if (value == null)
                value = Activator.CreateInstance(valueType);

            group = new PropertyGroup(domain, parent, valueType, value);
        }

        public override void OnGUI()
        {
            base.OnGUI();

            EditorHelper.Foldout(ref showGroup, Name, () => group.OnGUI(), LabelClick);
        }
    }
}
