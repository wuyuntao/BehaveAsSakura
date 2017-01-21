using BehaveAsSakura.Tasks;

namespace BehaveAsSakura.Editor
{
    public class TaskPropertyDescEvent : EditorEvent
    {
        public ITaskDesc CustomDesc { get; set; }

        public TaskPropertyDescEvent(string id)
            : base(id)
        {
        }
    }
}
