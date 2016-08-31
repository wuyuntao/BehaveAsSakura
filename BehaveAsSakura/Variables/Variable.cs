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

		public byte GetByte(Task task)
		{
			throw new NotImplementedException();
		}

		public byte GetSByte(Task task)
		{
			throw new NotImplementedException();
		}

		public short GetShort(Task task)
		{
			throw new NotImplementedException();
		}

		public ushort GetUShort(Task task)
		{
			throw new NotImplementedException();
		}

		public int GetInt(Task task)
		{
			throw new NotImplementedException();
		}

		public uint GetUInt(Task task)
		{
            return 3000;
		}

		public long GetLong(Task task)
		{
			throw new NotImplementedException();
		}

		public ulong GetULong(Task task)
		{
			throw new NotImplementedException();
		}

		public float GetFloat(Task task)
		{
			throw new NotImplementedException();
		}

		public double GetDouble(Task task)
		{
			throw new NotImplementedException();
		}

		public string GetString(Task task)
		{
			throw new NotImplementedException();
		}
	}
}
