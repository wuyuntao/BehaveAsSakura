using System.Collections.Generic;

namespace BehaveAsSakura.Editor
{
    class EditorRepository
    {
        public readonly Dictionary<string, EditorState> States = new Dictionary<string, EditorState>();
    }
}
