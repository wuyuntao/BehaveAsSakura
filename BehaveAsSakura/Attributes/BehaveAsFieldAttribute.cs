using System;

namespace BehaveAsSakura.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BehaveAsFieldAttribute : Attribute, IComparable, IComparable<BehaveAsFieldAttribute>
    {
        public BehaveAsFieldAttribute(int tag)
        {
            Tag = tag;
            IsRequired = true;
        }

        public int Tag { get; private set; }

        public bool IsRequired { get; set; }

        #region IComparable

        public int CompareTo(object obj)
        {
            var attr = obj as BehaveAsFieldAttribute;
            if (attr == null)
                return 1;

            return CompareTo(attr);
        }

        public int CompareTo(BehaveAsFieldAttribute other)
        {
            if (other == null)
                return 1;

            return Tag.CompareTo(other.Tag);
        }

        #endregion
    }
}
