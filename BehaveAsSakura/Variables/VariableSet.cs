using ProtoBuf;
using System.Collections.Generic;

namespace BehaveAsSakura.Variables
{
	[ProtoContract]
	class VariableSetProps
	{
		[ProtoMember( 1 )]
		public SortedList<string, VariableDesc> Variables;
	}

	public sealed class VariableSet
	{
		private SortedList<string, Variable> variables = new SortedList<string, Variable>();

		public Variable Get(string key)
		{
			Variable variable;
			variables.TryGetValue( key, out variable );
			return variable;
		}

		public Variable Set(string key, VariableType type, VariableSource source, string value)
		{
			return Set( key, new VariableDesc( type, source, value ) );
		}

		public Variable Set(string key, VariableDesc description)
		{
			var variable = new Variable( description );
			variables[key] = variable;
			return variable;
		}

		public Variable Remove(string key)
		{
			var variable = Get( key );
			if( variable != null )
				variables.Remove( key );
			return variable;
		}

		public void RemoveAll()
		{
			variables.Clear();
		}
	}
}
