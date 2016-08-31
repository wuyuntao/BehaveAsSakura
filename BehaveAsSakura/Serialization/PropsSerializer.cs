using BehaveAsSakura.Tasks;
using ProtoBuf;
using System.IO;

namespace BehaveAsSakura.Serialization
{
	[ProtoContract]
	class TaskPropsData
	{
		[ProtoMember( 1 )]
		public uint Id { get; set; }
		[ProtoMember( 2 )]
		public TaskState State { get; set; }

		[ProtoMember( 3 )]
		public TaskResult LastResult { get; set; }

		[ProtoMember( 4, IsRequired = false, DynamicType = true )]
		public object CustomProps { get; set; }
	}

	[ProtoContract]
	class BehaviorTreePropsData
	{
		[ProtoMember( 1 )]
		public TaskPropsData[] Tasks { get; set; }
	}

	static class PropsSerializer
	{
		public static byte[] Serialize(BehaviorTreePropsData tree)
		{
			using( var stream = new MemoryStream() )
			{
				Serializer.Serialize( stream, tree );

				return stream.ToArray();
			}
		}

		public static BehaviorTreePropsData Deserialize(byte[] data)
		{
			using( var stream = new MemoryStream( data ) )
			{
				return Serializer.Deserialize<BehaviorTreePropsData>( stream );
			}
		}
	}
}
