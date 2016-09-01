using BehaveAsSakura.Tasks;
using BehaveAsSakura.Variables;
using System.Collections.Generic;

namespace BehaveAsSakura.Tests
{
	class BehaviorTreeManagerOwner : IBehaviorTreeManagerOwner
	{
		BehaviorTreeDesc IBehaviorTreeLoader.LoadTree(string path)
		{
			var builder = new BehaviorTreeBuilder();

			switch( path )
			{
				case "Log":
					{
						builder.Leaf<LogTaskDesc>( d => d.Message = "Log" );
						break;
					}

				case "WaitTimer":
					{
						builder.Composite<SequenceTaskDesc>()
							.AppendChild( builder.Leaf<LogTaskDesc>(
									d => d.Message = "Start" ) )
							.AppendChild( builder.Leaf<WaitTimerTaskDesc>(
									d => d.Time = new VariableDesc( VariableType.UInteger, VariableSource.LiteralConstant, "3000" ) ) )
							.AppendChild( builder.Leaf<LogTaskDesc>( d =>
									d.Message = "CheckPoint1" ) )
							.AppendChild( builder.Leaf<WaitTimerTaskDesc>(
									d => d.Time = new VariableDesc( VariableType.UInteger, VariableSource.TreeOwnerProperty, "uint.3000" ) ) )
							.AppendChild( builder.Leaf<LogTaskDesc>( d =>
									d.Message = "CheckPoint2" ) )
							.AppendChild( builder.Leaf<WaitTimerTaskDesc>(
									d => d.Time = new VariableDesc( VariableType.UInteger, VariableSource.GlobalConstant, "uint.5000" ) ) )
							.AppendChild( builder.Leaf<LogTaskDesc>( d =>
									d.Message = "End" ) );
						break;
					}

				default:
					throw new KeyNotFoundException( path );
			}

			return builder.Build();
		}

		object IVariableContainer.GetValue(string key)
		{
			switch( key )
			{
				case "uint.5000":
					return 5000u;

				default:
					throw new KeyNotFoundException( key );
			}
		}
	}
}
