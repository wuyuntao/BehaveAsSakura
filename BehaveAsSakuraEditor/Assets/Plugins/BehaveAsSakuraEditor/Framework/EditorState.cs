using UnityEngine;

namespace BehaveAsSakura.Editor
{
    public abstract class EditorState : ScriptableObject
    {
        public delegate void EventAppliedHandler(EditorState state, EditorEvent e);

        public event EventAppliedHandler OnEventApplied;

        public string Id { get; private set; }

        public EditorDomain Domain { get; private set; }

        public EditorRepository Repository { get { return Domain.Repository; } }

        public EditorCommandHandler CommandHandler { get { return Domain.CommandHandler; } }

        public static T CreateState<T>(EditorDomain domain, string id)
            where T : EditorState, new()
        {
            var state = CreateInstance<T>();
            state.Domain = domain;
            state.Id = id;

            return state;
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
        }
    }
}
