using BehaveAsSakura.Tasks;
using System;

namespace BehaveAsSakura.Variables
{
	public sealed class Variable
	{
		private VariableDesc description;
		private object cachedValue;

		public Variable(VariableDesc description)
		{
			this.description = description;
		}

		public object GetValue(Task task)
		{
			throw new NotImplementedException();
		}
	}
}
