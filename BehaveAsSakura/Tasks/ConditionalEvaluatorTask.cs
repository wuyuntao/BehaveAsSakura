using BehaveAsSakura.Utils;
using BehaveAsSakura.Variables;
using ProtoBuf;
using System;

namespace BehaveAsSakura.Tasks
{
    [ProtoContract]
    public class ConditionalEvaluatorTaskDesc : ITaskDesc
    {
        [ProtoMember(1)]
        public VariableDesc Left { get; set; }

        [ProtoMember(2)]
        public ComparisonOperator Operator { get; set; }

        [ProtoMember(3)]
        public VariableDesc Right { get; set; }

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

            return description.Operator.Match(leftValue.CompareTo(rightValue)) ? TaskResult.Success : TaskResult.Failure;
        }
    }
}