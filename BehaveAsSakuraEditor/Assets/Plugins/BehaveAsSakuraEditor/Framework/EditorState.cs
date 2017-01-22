using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class EditorState : EditorObject
    {
        public event EventAppliedHandler OnEventApplied;

        public EditorDomain Domain { get; private set; }

        public EditorRepository Repository { get { return Domain.Repository; } }

        public EditorCommandHandler CommandHandler { get { return Domain.CommandHandler; } }

        protected EditorState(EditorDomain domain, string id)
            : base(id)
        {
            Domain = domain;
        }

        public override string ToString()
        {
            return Id;
        }

        public virtual void ApplyEvent(EditorEvent e)
        {
            Logger.Debug("'{0}' Apply event '{1}'", this, e);

            if (OnEventApplied != null)
                OnEventApplied(this, e);

            Domain.EventApplied(this, e);
        }
    }
}
