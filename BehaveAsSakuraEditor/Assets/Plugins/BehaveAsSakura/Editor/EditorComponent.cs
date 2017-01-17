using System.Collections.Generic;

namespace BehaveAsSakura.Editor
{
    abstract class EditorComponent
    {
        public readonly EditorComponent Parent;
        public readonly List<EditorComponent> Children = new List<EditorComponent>();

        protected EditorComponent(EditorComponent parent)
        {
            Parent = parent;
        }

        public virtual void OnGUI()
        {
            foreach (var child in Children)
            {
                child.OnGUI();
            }
        }
    }
}
