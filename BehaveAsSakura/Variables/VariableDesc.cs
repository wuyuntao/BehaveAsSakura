using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Variables
{
    public enum VariableType : byte
    {
        Byte,
        SByte,
        Short,
        UShort,
        Integer,
        UInteger,
        Long,
        ULong,
        Float,
        Double,
        String,
    }

    public enum VariableSource : byte
    {
        GlobalConstant,
        TreeOwnerProperty,
        TaskSharedVariable,
        LiteralConstant,
    }

    [BehaveAsContract]
    public class VariableDesc
    {
        [BehaveAsMember(1)]
        public VariableType Type { get; set; }

        [BehaveAsMember(2)]
        public VariableSource Source { get; set; }

        [BehaveAsMember(3)]
        public string Value { get; set; }

        public VariableDesc(VariableType type, VariableSource source, string value)
        {
            Type = type;
            Source = source;
            Value = value;
        }

        public VariableDesc() { }
    }
}
