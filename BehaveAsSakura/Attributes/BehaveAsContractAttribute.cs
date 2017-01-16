using System;

namespace BehaveAsSakura.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BehaveAsContractAttribute : Attribute
    {
        public BehaveAsContractAttribute()
        {
        }
    }
}
