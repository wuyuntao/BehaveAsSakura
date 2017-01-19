using BehaveAsSakura.Attributes;
using BehaveAsSakura.Tasks;
using System.Collections.Generic;

namespace BehaveAsSakura.Tests
{
    [BehaveAsEnum]
    public enum TestEnum1 : sbyte
    {
        Value1 = 0,
        Value3 = 3,
        Value7 = 7,
    }

    [BehaveAsTable]
    public sealed class TestData1
    {
        [BehaveAsEnum]
        public enum TestEnum2 : uint
        {
            Value1 = 0,
            Value3 = 3,
            Value7 = 7,
        }

        [BehaveAsField(1)]
        public string StringValue { get; set; }

        [BehaveAsField(2)]
        public bool BooleanValue { get; set; }

        [BehaveAsField(3)]
        public float FloatValue { get; set; }

        [BehaveAsField(4)]
        public TestEnum1 EnumValue { get; set; }

        [BehaveAsField(5)]
        public TestEnum2 EmbeddedEnumValue { get; set; }

        [BehaveAsField(6)]
        public TestData2 NestedValue { get; set; }


        [BehaveAsField(11)]
        public string[] StringArray { get; set; }

        [BehaveAsField(12)]
        public bool[] BooleanArray { get; set; }

        [BehaveAsField(13)]
        public ulong[] ULongArray { get; set; }

        [BehaveAsField(14)]
        public TestEnum1[] EnumArray { get; set; }

        [BehaveAsField(15)]
        public TestEnum2[] EmbeddedEnumArray { get; set; }

        [BehaveAsField(16)]
        public TestData2[] NestedClassArray { get; set; }


        [BehaveAsField(21)]
        public List<string> StringList { get; set; }

        [BehaveAsField(22)]
        public List<bool> BooleanList { get; set; }

        [BehaveAsField(23)]
        public List<uint> UIntList { get; set; }

        [BehaveAsField(24)]
        public List<TestEnum1> EnumList { get; set; }

        [BehaveAsField(25)]
        public List<TestEnum2> EmbeddedEnumList { get; set; }

        [BehaveAsField(26)]
        public List<TestData2> NestedClassList { get; set; }

    }

    [BehaveAsTable]
    public class TestData2
    {
        [BehaveAsField(1)]
        public long LongValue { get; set; }

        [BehaveAsField(2)]
        public ulong ULongValue { get; set; }
    }

    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 30)]
    public sealed class DataTypeTestTaskDesc : ILeafTaskDesc
    {
        [BehaveAsField(1)]
        public string StringValue { get; set; }

        [BehaveAsField(2)]
        public bool BooleanValue { get; set; }

        [BehaveAsField(3)]
        public byte ByteValue { get; set; }

        [BehaveAsField(4)]
        public TestEnum1 EnumValue { get; set; }

        [BehaveAsField(5)]
        public TestData1.TestEnum2 EmbeddedEnumValue { get; set; }

        [BehaveAsField(6)]
        public TestData1 NestedValue { get; set; }


        [BehaveAsField(11)]
        public string[] StringArray { get; set; }

        [BehaveAsField(12)]
        public bool[] BooleanArray { get; set; }

        [BehaveAsField(13)]
        public short[] ShortArray { get; set; }

        [BehaveAsField(14)]
        public TestEnum1[] EnumArray { get; set; }

        [BehaveAsField(15)]
        public TestData1.TestEnum2[] EmbeddedEnumArray { get; set; }

        [BehaveAsField(16)]
        public TestData1[] NestedClassArray { get; set; }


        [BehaveAsField(21)]
        public List<string> StringList { get; set; }

        [BehaveAsField(22)]
        public List<bool> BooleanList { get; set; }

        [BehaveAsField(23)]
        public List<int> ShortList { get; set; }

        [BehaveAsField(24)]
        public List<TestEnum1> EnumList { get; set; }

        [BehaveAsField(25)]
        public List<TestData1.TestEnum2> EmbeddedEnumList { get; set; }

        [BehaveAsField(26)]
        public List<TestData1> NestedClassList { get; set; }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new DataTypeTestTask(tree, parentTask, id, this);
        }
    }

    class DataTypeTestTask : LeafTask
    {
        public DataTypeTestTask(BehaviorTree tree, Task parentTask, uint id, DataTypeTestTaskDesc description)
            : base(tree, parentTask, id, description)
        {
        }

        protected override TaskResult OnUpdate()
        {
            return TaskResult.Success;
        }
    }
}
