using BehaveAsSakura.Serialization;
using BehaveAsSakura.Tasks;
using BehaveAsSakura.Variables;
using NUnit.Framework;

namespace BehaveAsSakura.Tests
{
    [TestFixture]
    class BehaviorTreeBuilderTests
    {
        [Test]
        public void TestSerialization()
        {
            var tree1 = BuildTreeDesc();

            var data1 = BehaviorTreeSerializer.SerializeDesc(tree1);
            Assert.IsTrue(data1.Length > 0);

            var tree2 = BehaviorTreeSerializer.DeserializeDesc(data1);

            var task1 = tree2.Tasks[0];
            Assert.IsTrue(task1.CustomDesc is SequenceTaskDesc);

            var task2 = tree2.Tasks[1];
            Assert.IsTrue(task2.CustomDesc is LogTaskDesc);

            var task3 = tree2.Tasks[2];
            Assert.IsTrue(task3.CustomDesc is WaitTimerTaskDesc);
        }

        static BehaviorTreeDesc BuildTreeDesc()
        {
            var builder = new BehaviorTreeBuilder();
            builder.Composite<SequenceTaskDesc>()
                .AppendChild(builder.Leaf<LogTaskDesc>(
                        d => d.Message = "Start"))
                .AppendChild(builder.Leaf<WaitTimerTaskDesc>(
                        d => d.Time = new VariableDesc(VariableType.UInteger, VariableSource.LiteralConstant, "3000")))
                .AppendChild(builder.Leaf<LogTaskDesc>(d =>
                        d.Message = "End"));
            return builder.Build();
        }
    }
}
