using System;

namespace BehaveAsSakura.Utils
{
    public static class Validation
    {
        public static void NotNull<T>(T value, string name)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        public static void IsEnumDefined<T>(T value, string name)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"{typeof(T).FullName} is not enum");

            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentException($"{name} is not defined in {typeof(T).FullName}");
        }

        public static void NotEmpty(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(name);
        }
    }
}
