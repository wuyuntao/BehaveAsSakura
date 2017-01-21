using System;

namespace BehaveAsSakura.Utils
{
    static class ValidationExtension
    {
        public static void ValidateNotNull<T>(this T value, string name)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }

        public static void ValidateIsEnumDefined<T>(this T value, string name)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException($"{typeof(T).FullName} is not enum");

            if (!Enum.IsDefined(typeof(T), value))
                throw new ArgumentException($"{name} is not defined in {typeof(T).FullName}");
        }

        public static void ValidateNotEmpty(this string value, string name)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(name);
        }

        public static void ValidateNotEmpty<T>(this T[] values, string name)
            where T : class
        {
            if (values == null || values.Length == 0)
                throw new ArgumentNullException(name);
        }
    }
}
