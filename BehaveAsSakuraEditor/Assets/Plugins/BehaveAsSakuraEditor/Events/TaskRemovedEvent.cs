namespace BehaveAsSakura.Editor
{
    public class TaskRemovedEvent : EditorEvent
    {
        public TaskRemovedEvent(string id)
            : base(id)
        {
        }
    }
}
