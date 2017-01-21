using BehaveAsSakura.Attributes;
using BehaveAsSakura.Utils;
using BehaveAsSakura.Variables;
using System;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 1)]
    public class ConditionalEvaluatorTaskDesc : ILeafTaskDesc
    {
        [BehaveAsField(1)]
        public VariableDesc Left { get; set; }

        [BehaveAsField(2)]
        public ComparisonOperator Op { get; set; }

        [BehaveAsField(3)]
        public VariableDesc Right { get; set; }

        void ITaskDesc.Validate()
        {
            Left.ValidateNotNull(nameof(Left));
            Left.Validate();

            Op.ValidateIsEnumDefined(nameof(Op));

            Right.ValidateNotNull(nameof(Right));
            Right.Validate();
        }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new ConditionalEvaluatorTask(tree, parentTask, id, this);
        }
    }

    class ConditionalEvaluatorTask : LeafTask
    {
        ConditionalEvaluatorTaskDesc description;
        Variable leftVariable;
        Variable rightVariable;

        public ConditionalEvaluatorTask(BehaviorTree tree, Task parentTask, uint id, ConditionalEvaluatorTaskDesc description)
            : base(tree, parentTask, id, description)
        {
            this.description = description;
            leftVariable = new Variable(description.Left);
            rightVariable = new Variable(description.Right);
        }

        protected override TaskResult OnUpdate()
        {
            var leftValue = leftVariable.GetValue(this) as IComparable;
            var rightValue = rightVariable.GetValue(this) as IComparable;

            if (leftValue == null || rightValue == null)
            {
                LogError("{0}: Failed to get value from variables. Left: {0} = {1}, Right: {2} = {3}",
                        leftVariable, leftValue, rightVariable, rightValue);
                return TaskResult.Failure;
            }

            return description.Op.Match(leftValue.CompareTo(rightValue)) ? TaskResult.Success : TaskResult.Failure;
        }
    }
}