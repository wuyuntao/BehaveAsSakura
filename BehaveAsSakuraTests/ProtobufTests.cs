using NUnit.Framework;
using ProtoBuf;
using System.IO;

namespace BehaveAsSakura.Tests
{
    [ProtoContract]
    [ProtoInclude(2, typeof(SomeDerivedClass))]
    class SomeBaseClass
    {
        [ProtoMember(1)]
        public int Field1;
    }

    [ProtoContract]
    class SomeDerivedClass : SomeBaseClass
    {
        [ProtoMember(2)]
        public int Field2;
    }

    [TestFixture]
    public class ProtobufTests
    {
        [Test]
        public void Serialize()
        {
            var cls1 = new SomeBaseClass() { Field1 = 100 };
            var cls2 = new SomeDerivedClass() { Field1 = 100, Field2 = 200 };

            using (var stream1 = new MemoryStream())
            {
                Serializer.Serialize(stream1, cls1);

                using (var stream2 = new MemoryStream(stream1.ToArray()))
                {
                    var cls3 = Serializer.Deserialize<SomeBaseClass>(stream2);

                    Assert.AreEqual(cls1.Field1, cls3.Field1);
                }
            }

            using (var stream1 = new MemoryStream())
            {
                Serializer.Serialize(stream1, cls2);

                using (var stream2 = new MemoryStream(stream1.ToArray()))
                {
                    var cls4 = Serializer.Deserialize<SomeDerivedClass>(stream2);

                    Assert.AreEqual(cls2.Field1, cls4.Field1);
                    Assert.AreEqual(cls2.Field2, cls4.Field2);
                }
            }
        }
    }
}
