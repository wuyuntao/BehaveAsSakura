using BehaveAsSakura.Tasks;

namespace BehaveAsSakura.Editor
{
    public class ChangeTaskDescCommand : EditorCommand
    {
        public ITaskDesc CustomDesc { get; set; }

        public ChangeTaskDescCommand(string id)
            : base(id)
        {
        }
    }
}
