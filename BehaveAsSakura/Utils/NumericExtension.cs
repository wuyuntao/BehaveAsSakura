namespace BehaveAsSakura.Utils
{
    static class NumericExtension
    {
        public static uint SafeAdd(this uint value, uint addValue)
        {
            var r = value + addValue;
            if (r < value || r < addValue)
                return uint.MaxValue;
            else
                return r;
        }

        public static uint SafeSub(this uint value, uint subValue)
        {
            if (value <= subValue)
                return 0;
            else
                return value - subValue;
        }

        public static float SafeAdd(this float value, float addValue)
        {
            var r = value + addValue;
            if (r < value || r < addValue)
                return uint.MaxValue;
            else
                return r;
        }

        public static float SafeSub(this float value, float subValue)
        {
            if (value <= subValue)
                return 0;
            else
                return value - subValue;
        }
    }
}