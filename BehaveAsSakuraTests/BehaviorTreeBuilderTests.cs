using BehaveAsSakura.Tasks;
using BehaveAsSakura.Variables;
using NUnit.Framework;
using System.IO;

namespace BehaveAsSakura.Tests
{
    [TestFixture]
    class BehaviorTreeBuilderTests
    {
        [Test]
        public void TestSerialization()
        {
            using (var ms1 = new MemoryStream())
            {
                var tree1 = BuildTreeDesc();
                //Serializer.Serialize(ms1, tree1);

                var data = ms1.ToArray();
                Assert.IsTrue(data.Length > 0);

                //using (var ms2 = new MemoryStream(data))
                //{
                //    var tree2 = Serializer.Deserialize<BehaviorTreeDesc>(ms2);
                //}
            }
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
