using BehaveAsSakura.Serialization;
using UnityEditor;

namespace BehaveAsSakura
{
    [InitializeOnLoad]
    class BehaveAsSakuraInitializer
    {
        static BehaveAsSakuraInitializer()
        {
            BehaviorTreeSerializer.Initialize(new FlatBuffersSerializer());
        }
    }
}
