using System;

namespace BehaveAsSakura.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BehaveAsMemberAttribute : Attribute, IComparable, IComparable<BehaveAsMemberAttribute>
    {
        public BehaveAsMemberAttribute(int tag)
        {
            Tag = tag;
            IsRequired = true;
        }

        public int Tag { get; private set; }

        public bool IsRequired { get; set; }

        #region IComparable

        public int CompareTo(object obj)
        {
            var attr = obj as BehaveAsMemberAttribute;
            if (attr == null)
                return 1;

            return CompareTo(attr);
        }

        public int CompareTo(BehaveAsMemberAttribute other)
        {
            if (other == null)
                return 1;

            return Tag.CompareTo(other.Tag);
        }

        #endregion
    }
}
