using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaveAsSakura.Serialization
{
	class TaskDescData
	{
	}

	[ProtoContract]
	class TaskDescWrapperData
	{
		[ProtoMember( 1 )]
		public int Tag;

		[ProtoMember( 2, IsPacked = true )]
		public byte[] Data;
	}

	class BehaviorTreeDescData
	{
	}
}
