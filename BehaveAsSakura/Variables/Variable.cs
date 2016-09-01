using BehaveAsSakura.Tasks;
using System;

namespace BehaveAsSakura.Variables
{
	public interface IVariableContainer
	{
		object GetValue(string key);
	}

	public sealed class Variable
	{
		private VariableDesc description;

		public Variable(VariableDesc description)
		{
			this.description = description;
		}

		public object GetValue(Task task)
		{
			switch( description.Source )
			{
				case VariableSource.GlobalConstant:
					return GetValue_Container( task.Tree.TreeManager.Owner );

				case VariableSource.TreeOwnerProperty:
					return GetValue_Container( task.Tree.Owner );

				case VariableSource.TaskSharedVariable:
					return GetValue_TaskSharedVariable( task );

				case VariableSource.LiteralConstant:
					return GetValue_LiteralConstant();

				default:
					throw new NotSupportedException( description.Source.ToString() );
			}
		}

		public byte GetByte(Task task)
		{
			return (byte)GetValue( task );
		}

		public sbyte GetSByte(Task task)
		{
			return (sbyte)GetValue( task );
		}

		public short GetShort(Task task)
		{
			return (short)GetValue( task );
		}

		public ushort GetUShort(Task task)
		{
			return (ushort)GetValue( task );
		}

		public int GetInt(Task task)
		{
			return (int)GetValue( task );
		}

		public uint GetUInt(Task task)
		{
			return (uint)GetValue( task );
		}

		public long GetLong(Task task)
		{
			return (long)GetValue( task );
		}

		public ulong GetULong(Task task)
		{
			return (ulong)GetValue( task );
		}

		public float GetFloat(Task task)
		{
			return (float)GetValue( task );
		}

		public double GetDouble(Task task)
		{
			return (double)GetValue( task );
		}

		public string GetString(Task task)
		{
			return (string)GetValue( task );
		}

		#region Helpers

		object GetValue_Container(IVariableContainer container)
		{
			return container.GetValue( description.Value );
		}

		object GetValue_LiteralConstant()
		{
			switch( description.Type )
			{
				case VariableType.Byte:
					return byte.Parse( description.Value );

				case VariableType.SByte:
					return sbyte.Parse( description.Value );

				case VariableType.Short:
					return short.Parse( description.Value );

				case VariableType.UShort:
					return ushort.Parse( description.Value );

				case VariableType.Integer:
					return int.Parse( description.Value );

				case VariableType.UInteger:
					return uint.Parse( description.Value );

				case VariableType.Long:
					return long.Parse( description.Value );

				case VariableType.ULong:
					return ulong.Parse( description.Value );

				case VariableType.Float:
					return float.Parse( description.Value );

				case VariableType.Double:
					return double.Parse( description.Value );

				case VariableType.String:
					return description.Value;

				default:
					throw new NotSupportedException( description.Type.ToString() );
			}
		}

		object GetValue_TaskSharedVariable(Task task)
		{
			var variable = task.GetSharedVariable( description.Value );
			if( variable == null )
				return null;

			return variable.GetValue( task );
		}

		#endregion
	}
}
