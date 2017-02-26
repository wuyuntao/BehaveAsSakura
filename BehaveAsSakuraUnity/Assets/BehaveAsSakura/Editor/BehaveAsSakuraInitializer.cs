using BehaveAsSakura.Editor;
using BehaveAsSakura.Serialization;
using UnityEditor;

namespace BehaveAsSakura
{
    [InitializeOnLoad]
    class BehaveAsSakuraInitializer
    {
        static BehaveAsSakuraInitializer()
        {
            // Use translation in another language
            //I18n.SetLanguage("zh_CN");

            BehaviorTreeSerializer.Initialize(new FlatBuffersSerializer());
        }
    }
}
