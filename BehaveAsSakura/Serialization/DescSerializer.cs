using ProtoBuf;
using System.IO;

namespace BehaveAsSakura.Serialization
{
	[ProtoContract]
	class TaskDescData
	{
		[ProtoMember( 1 )]
		public uint Id { get; set; }

		[ProtoMember( 2, IsRequired = false )]
		public string Name { get; set; }

		[ProtoMember( 3, IsRequired = false )]
		public string Comment { get; set; }

		[ProtoMember( 4, DynamicType = true )]
		public object CustomDesc { get; set; }
	}

	[ProtoContract]
	class DecoratorTaskDescData : TaskDescData
	{
		[ProtoMember( 1 )]
		public uint ChildTask { get; set; }
	}

	[ProtoContract]
	class CompositeTaskDescData : TaskDescData
	{
		[ProtoMember( 1 )]
		public uint[] ChildTasks { get; set; }
	}

	[ProtoContract]
	class BehaviorTreeDescData
	{
		[ProtoMember( 1 )]
		public TaskDescData[] Tasks { get; set; }

		[ProtoMember( 2 )]
		public uint RootTaskId { get; set; }
	}

	static class DescSerializer
	{
		public static byte[] Serialize(BehaviorTreeDescData tree)
		{
			using( var stream = new MemoryStream() )
			{
				Serializer.Serialize( stream, tree );

				return stream.ToArray();
			}
		}

		public static BehaviorTreeDescData Deserialize(byte[] data)
		{
			using( var stream = new MemoryStream( data ) )
			{
				return Serializer.Deserialize<BehaviorTreeDescData>( stream );
			}
		}
	}
}
