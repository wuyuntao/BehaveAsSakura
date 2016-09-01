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
							.AppendChild( builder.Leaf<DumpLogTaskDesc>(
									d => d.Text = "Start" ) )
							.AppendChild( builder.Leaf<WaitTimerTaskDesc>(
									d => d.Time = new VariableDesc( VariableType.UInteger, VariableSource.LiteralConstant, "3000" ) ) )
							.AppendChild( builder.Leaf<DumpLogTaskDesc>( d =>
									d.Text = "CheckPoint1" ) )
							.AppendChild( builder.Leaf<WaitTimerTaskDesc>(
									d => d.Time = new VariableDesc( VariableType.UInteger, VariableSource.TreeOwnerProperty, "uint.3000" ) ) )
							.AppendChild( builder.Leaf<DumpLogTaskDesc>( d =>
									d.Text = "CheckPoint2" ) )
							.AppendChild( builder.Leaf<WaitTimerTaskDesc>(
									d => d.Time = new VariableDesc( VariableType.UInteger, VariableSource.GlobalConstant, "uint.5000" ) ) )
							.AppendChild( builder.Leaf<DumpLogTaskDesc>( d =>
									d.Text = "End" ) );
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
