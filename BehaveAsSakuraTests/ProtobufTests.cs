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

        [ProtoMember(3, IsRequired = false)]
        public SomeDerivedClass[] Children;
    }

    [ProtoContract]
    class SomeWrapperClass
    {
        [ProtoMember(1)]
        public SomeBaseClass Child;

        [ProtoMember(2, DynamicType = true)]
        public object[] Children;

        [ProtoMember(3)]
        public SomeBaseClass[] InheritedChildren;
    }

    [TestFixture]
    class ProtobufTests
    {
        [Test]
        public void SerializeInheritance()
        {
            var cls1 = new SomeBaseClass() { Field1 = 100 };
            var cls2 = new SomeDerivedClass()
            {
                Field1 = 100,
                Field2 = 200,
                Children = new[]
                {
                    new SomeDerivedClass() { Field1 = 300, Field2 = 400 },
                    new SomeDerivedClass() { Field1 = 500, Field2 = 600 },
                }
            };

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
                    Assert.AreEqual(cls2.Children.Length, cls4.Children.Length);
                }
            }
        }

        [Test]
        public void SerializeDynamicType()
        {
            var cls1 = new SomeWrapperClass()
            {
                Child = new SomeDerivedClass() { Field1 = 1, Field2 = 2 },
                Children = new[]
                {
                    new SomeDerivedClass() { Field1 = 3, Field2 = 4 },
                    new SomeBaseClass() { Field1 = 5 },
                },
                InheritedChildren = new[]
                {
                    new SomeDerivedClass() { Field1 = 6, Field2 = 7 },
                    new SomeBaseClass() { Field1 = 8 },
                }
            };

            using (var stream1 = new MemoryStream())
            {
                Serializer.Serialize(stream1, cls1);

                using (var stream2 = new MemoryStream(stream1.ToArray()))
                {
                    var cls3 = Serializer.Deserialize<SomeWrapperClass>(stream2);

                    Assert.AreEqual(cls1.Child.Field1, cls3.Child.Field1);
                    Assert.IsTrue(cls3.Child is SomeDerivedClass);

                    Assert.AreEqual(cls1.Children.Length, cls3.Children.Length);

                    Assert.IsTrue(cls3.Children[0] is SomeDerivedClass);
                    Assert.AreEqual(((SomeDerivedClass)cls1.Children[0]).Field1, ((SomeDerivedClass)cls3.Children[0]).Field1);
                    Assert.AreEqual(((SomeDerivedClass)cls1.Children[0]).Field2, ((SomeDerivedClass)cls3.Children[0]).Field2);

                    Assert.IsFalse(cls3.Children[1] is SomeDerivedClass);
                    Assert.AreEqual(((SomeBaseClass)cls1.Children[1]).Field1, ((SomeBaseClass)cls3.Children[1]).Field1);

                    Assert.AreEqual(cls1.InheritedChildren.Length, cls3.InheritedChildren.Length);

                    Assert.IsTrue(cls3.InheritedChildren[0] is SomeDerivedClass);
                    Assert.AreEqual(((SomeDerivedClass)cls1.InheritedChildren[0]).Field1, ((SomeDerivedClass)cls3.InheritedChildren[0]).Field1);
                    Assert.AreEqual(((SomeDerivedClass)cls1.InheritedChildren[0]).Field2, ((SomeDerivedClass)cls3.InheritedChildren[0]).Field2);

                    Assert.IsFalse(cls3.InheritedChildren[1] is SomeDerivedClass);
                    Assert.AreEqual(((SomeBaseClass)cls1.InheritedChildren[1]).Field1, ((SomeBaseClass)cls3.InheritedChildren[1]).Field1);
                }
            }
        }
    }
}
