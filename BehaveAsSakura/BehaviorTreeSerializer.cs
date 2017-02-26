using System;

namespace BehaveAsSakura
{
    public interface IBehaviorTreeSerializer
    {
        byte[] SerializeDesc(BehaviorTreeDesc desc);

        BehaviorTreeDesc DeserializeDesc(byte[] data);

        byte[] SerializeProps(BehaviorTreeProps props);

        BehaviorTreeProps DeserializeProps(byte[] data);
    }

    public static class BehaviorTreeSerializer
    {
        private static IBehaviorTreeSerializer instance;
        private static object instanceLock = new object();

        public static void Initialize(IBehaviorTreeSerializer serializer)
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                        instance = serializer;
                }
            }
        }

        public static byte[] SerializeDesc(BehaviorTreeDesc desc)
        {
            CheckInstanceInitialized();

            return instance.SerializeDesc(desc);
        }

        public static BehaviorTreeDesc DeserializeDesc(byte[] data)
        {
            CheckInstanceInitialized();

            return instance.DeserializeDesc(data);
        }

        public static byte[] SerializeProps(BehaviorTreeProps props)
        {
            CheckInstanceInitialized();

            return instance.SerializeProps(props);
        }

        public static BehaviorTreeProps DeserializeProps(byte[] data)
        {
            CheckInstanceInitialized();

            return instance.DeserializeProps(data);
        }

        private static void CheckInstanceInitialized()
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                        throw new ArgumentException("Serializer not initialized");
                }
            }
        }
    }
}
