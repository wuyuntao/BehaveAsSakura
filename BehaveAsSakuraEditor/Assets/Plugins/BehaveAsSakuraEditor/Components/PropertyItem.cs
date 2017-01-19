using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BehaveAsSakura.Editor
{
    public abstract class PropertyItem : EditorComponent
    {
        protected PropertyItem(EditorDomain domain, PropertyGroup parent, string id)
            : base(domain, parent, id)
        { }
    }
}
