using BehaveAsSakura.Attributes;
using System;

namespace BehaveAsSakura.Utils
{
    [BehaveAsEnum]
    public enum ComparisonOperator : byte
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }

    static class ComparisonOperatorHelper
    {
        public static string Format(this ComparisonOperator op)
        {
            switch (op)
            {
                case ComparisonOperator.Equal:
                    return "==";
                case ComparisonOperator.NotEqual:
                    return "!=";
                case ComparisonOperator.GreaterThan:
                    return ">";
                case ComparisonOperator.GreaterThanOrEqual:
                    return ">=";
                case ComparisonOperator.LessThan:
                    return "<";
                case ComparisonOperator.LessThanOrEqual:
                    return "<=";
                default:
                    throw new NotSupportedException(op.ToString());
            }
        }

        public static bool Match(this ComparisonOperator op, int result)
        {
            switch (op)
            {
                case ComparisonOperator.Equal:
                    return result == 0;
                case ComparisonOperator.NotEqual:
                    return result != 0;
                case ComparisonOperator.GreaterThan:
                    return result > 0;
                case ComparisonOperator.GreaterThanOrEqual:
                    return result >= 0;
                case ComparisonOperator.LessThan:
                    return result < 0;
                case ComparisonOperator.LessThanOrEqual:
                    return result <= 0;
                default:
                    throw new NotSupportedException(op.ToString());
            }
        }
    }
}