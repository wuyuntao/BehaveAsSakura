using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class EditorComponent
    {
        public EditorDomain Domain { get; private set; }

        public EditorComponent Parent { get; private set; }

        public List<EditorComponent> Children { get; private set; }

        protected EditorComponent(EditorDomain domain, EditorComponent parent)
        {
            Domain = domain;
            Parent = parent;
            Children = new List<EditorComponent>();
        }

        public virtual void OnGUI()
        {
            foreach (var child in Children)
                child.OnGUI();
        }

        public virtual bool OnMouseDown(Event e)
        {
            return Children.Any(c => c.OnMouseDown(e));
        }

        public virtual bool OnMouseUp(Event e)
        {
            return Children.Any(c => c.OnMouseUp(e));
        }
    }
}
