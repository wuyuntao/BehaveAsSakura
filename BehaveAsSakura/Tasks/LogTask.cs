using BehaveAsSakura.Variables;
using ProtoBuf;
using System;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public sealed class LogDesc : TaskDesc
	{
		[ProtoMember( 1 )]
		public string Message { get; set; }

		[ProtoMember( 2, IsRequired = false )]
		public VariableDesc[] MessageParameters { get; set; }
	}

	public sealed class LogTask : LeafTask
	{
		private LogDesc description;
		private Variable[] variables;

		public LogTask(BehaviorTree tree, Task parent, LogDesc description)
			: base( tree, parent, description, new TaskProps( description.Id ) )
		{
			this.description = description;

			if( description.MessageParameters != null )
				variables = Array.ConvertAll( description.MessageParameters, desc => new Variable( desc ) );
		}

		protected override TaskResult OnUpdate()
		{
			if( variables != null )
				LogInfo( description.Message, Array.ConvertAll( variables, v => v.GetValue( this ) ) );
			else
				LogInfo( description.Message );

			return TaskResult.Success;
		}
	}
}
