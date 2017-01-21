using System.Collections.Generic;
using System.Reflection;

namespace BehaveAsSakura.Editor
{
    public class ChangeTaskPropertyCommand : EditorCommand
    {
        public List<Item> Items { get; set; }

        public ChangeTaskPropertyCommand(string id)
            : base(id)
        {
            Items = new List<Item>();
        }

        public class Item
        {
            public PropertyInfo PropertyInfo { get; set; }

            public object Target { get; set; }

            public object Value { get; set; }
        }
    }
}
