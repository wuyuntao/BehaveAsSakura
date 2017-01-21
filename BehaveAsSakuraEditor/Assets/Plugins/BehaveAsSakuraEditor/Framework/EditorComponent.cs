using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class EditorComponent : EditorObject
    {
        public EditorDomain Domain { get; private set; }

        public EditorRepository Repository { get { return Domain.Repository; } }

        public EditorCommandHandler CommandHandler { get { return Domain.CommandHandler; } }

        public EditorComponent Parent { get; private set; }

        public List<EditorComponent> Children { get; private set; }

        protected EditorComponent(EditorDomain domain, EditorComponent parent, string id)
            : base(id)
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

        public virtual bool OnMouseDrag(Event e)
        {
            return Children.Any(c => c.OnMouseDrag(e));
        }

        public virtual bool OnMouseMove(Event e)
        {
            return Children.Any(c => c.OnMouseMove(e));
        }

        public virtual bool OnLayoutChange(Event e, Rect windowPosition)
        {
            return Children.Any(c => c.OnLayoutChange(e, windowPosition));
        }
    }
}
