using BehaveAsSakura.Utils;
using BehaveAsSakura.Variables;
using ProtoBuf;
using System;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class ConditionalEvaluatorTaskDesc : TaskDesc
	{
		[ProtoMember( 1 )]
		public VariableDesc Left { get; set; }

		[ProtoMember( 2 )]
		public ComparisonOperator Operator { get; set; }

		[ProtoMember( 3 )]
		public VariableDesc Right { get; set; }
	}

	public sealed class ConditionalEvaluatorTask : LeafTask
	{
		ConditionalEvaluatorTaskDesc m_template;
		Variable m_leftVariable;
		Variable m_rightVariable;

		public ConditionalEvaluatorTask(BehaviorTree tree, Task parent, ConditionalEvaluatorTaskDesc template)
			: base( tree, parent, template, new TaskProps( template.Id ) )
		{
			m_template = template;
			m_leftVariable = new Variable( m_template.Left );
			m_rightVariable = new Variable( m_template.Right );
		}

		protected override TaskResult OnUpdate()
		{
			var leftValue = m_leftVariable.GetValue( this ) as IComparable;
			var rightValue = m_rightVariable.GetValue( this ) as IComparable;

			if( leftValue == null || rightValue == null )
			{
				LogError( "{0}: Failed to get value from variables. Left: {0} = {1}, Right: {2} = {3}",
						m_leftVariable, leftValue, m_rightVariable, rightValue );
				return TaskResult.Failure;
			}

			return m_template.Operator.Match( leftValue.CompareTo( rightValue ) ) ? TaskResult.Success : TaskResult.Failure;
		}
	}
}