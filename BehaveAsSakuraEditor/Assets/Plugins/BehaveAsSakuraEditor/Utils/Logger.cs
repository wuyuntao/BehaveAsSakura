namespace BehaveAsSakura.Editor
{
    public static class Logger
    {
        public enum Level
        {
            Debug,
            Info,
            Warn,
            Error,
        }

        public static void Debug(string msg)
        {
            Log(Level.Debug, msg, null);
        }

        public static void Debug(string msg, params object[] args)
        {
            Log(Level.Debug, msg, args);
        }

        public static void Info(string msg)
        {
            Log(Level.Info, msg, null);
        }

        public static void Info(string msg, params object[] args)
        {
            Log(Level.Info, msg, args);
        }

        public static void Warn(string msg)
        {
            Log(Level.Warn, msg, null);
        }

        public static void Warn(string msg, params object[] args)
        {
            Log(Level.Warn, msg, args);
        }

        public static void Error(string msg)
        {
            Log(Level.Error, msg, null);
        }

        public static void Error(string msg, params object[] args)
        {
            Log(Level.Error, msg, args);
        }

        private static void Log(Level level, string msg, object[] args)
        {
            if (EditorConfiguration.LoggerLevel > level)
                return;

            switch (level)
            {
                case Level.Warn:
                    if (args != null)
                        UnityEngine.Debug.LogWarningFormat(msg, args);
                    else
                        UnityEngine.Debug.LogWarning(msg);
                    break;

                case Level.Error:
                    if (args != null)
                        UnityEngine.Debug.LogErrorFormat(msg, args);
                    else
                        UnityEngine.Debug.LogError(msg);
                    break;

                default:
                    if (args != null)
                        UnityEngine.Debug.LogFormat(msg, args);
                    else
                        UnityEngine.Debug.Log(msg);
                    break;
            }
        }
    }
}
