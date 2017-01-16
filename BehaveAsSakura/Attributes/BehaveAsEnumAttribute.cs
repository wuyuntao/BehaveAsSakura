using System;

namespace BehaveAsSakura.Attributes
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class BehaveAsEnumAttribute : Attribute
    {
        public BehaveAsEnumAttribute()
        {
        }
    }
}
