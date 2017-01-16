using BehaveAsSakura.Attributes;

namespace BehaveAsSakura.Variables
{
    [BehaveAsEnum]
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

    [BehaveAsEnum]
    public enum VariableSource : byte
    {
        GlobalConstant,
        TreeOwnerProperty,
        TaskSharedVariable,
        LiteralConstant,
    }

    [BehaveAsTable]
    public class VariableDesc
    {
        [BehaveAsField(1)]
        public VariableType Type { get; set; }

        [BehaveAsField(2)]
        public VariableSource Source { get; set; }

        [BehaveAsField(3)]
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
