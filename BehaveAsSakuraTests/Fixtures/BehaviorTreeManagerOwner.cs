using BehaveAsSakura.Tasks;
using BehaveAsSakura.Utils;
using BehaveAsSakura.Variables;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura.Tests
{
    class BehaviorTreeManagerOwner : IBehaviorTreeManagerOwner
    {
        BehaviorTreeDesc IBehaviorTreeLoader.LoadTree(string path)
        {
            var builder = new BehaviorTreeBuilder();

            switch (path)
            {
                case "Log":
                    {
                        builder.Leaf<LogTaskDesc>(d => d.Message = "Log");
                        break;
                    }

                case "WaitTimer":
                    {
                        builder.Composite<SequenceTaskDesc>()
                            .AppendChild(builder.Leaf<DumpLogTaskDesc>(
                                    d => d.Text = "Start"))
                            .AppendChild(builder.Leaf<WaitTimerTaskDesc>(
                                    d => d.Time = new VariableDesc(VariableType.UInteger, VariableSource.LiteralConstant, "3000")))
                            .AppendChild(builder.Leaf<DumpLogTaskDesc>(d =>
                                    d.Text = "CheckPoint1"))
                            .AppendChild(builder.Leaf<WaitTimerTaskDesc>(
                                    d => d.Time = new VariableDesc(VariableType.UInteger, VariableSource.TreeOwnerProperty, "uint.3000")))
                            .AppendChild(builder.Leaf<DumpLogTaskDesc>(d =>
                                    d.Text = "CheckPoint2"))
                            .AppendChild(builder.Leaf<WaitTimerTaskDesc>(
                                    d => d.Time = new VariableDesc(VariableType.UInteger, VariableSource.GlobalConstant, "uint.5000")))
                            .AppendChild(builder.Leaf<DumpLogTaskDesc>(d =>
                                    d.Text = "End"));
                        break;
                    }

                case "ConditionEvaluator1":
                    {
                        builder.Composite<SelectorTaskDesc>()
                            .AppendChild(builder.Leaf<ConditionalEvaluatorTaskDesc>(d =>
                                {
                                    d.Left = new VariableDesc(VariableType.UInteger, VariableSource.GlobalConstant, "uint.5000");
                                    d.Operator = ComparisonOperator.LessThan;
                                    d.Right = new VariableDesc(VariableType.UInteger, VariableSource.LiteralConstant, "100");
                                }))
                            .AppendChild(builder.Leaf<ConditionalEvaluatorTaskDesc>(d =>
                                {
                                    d.Left = new VariableDesc(VariableType.Float, VariableSource.GlobalConstant, "float.pi");
                                    d.Operator = ComparisonOperator.GreaterThan;
                                    d.Right = new VariableDesc(VariableType.Float, VariableSource.LiteralConstant, "4");
                                }))
                            .AppendChild(builder.Leaf<ConditionalEvaluatorTaskDesc>(d =>
                                {
                                    d.Left = new VariableDesc(VariableType.Float, VariableSource.GlobalConstant, "float.pi");
                                    d.Operator = ComparisonOperator.Equal;
                                    d.Right = new VariableDesc(VariableType.Float, VariableSource.GlobalConstant, "float.pi");
                                }));
                    }
                    break;

                default:
                    throw new KeyNotFoundException(path);
            }

            return builder.Build();
        }

        object IVariableContainer.GetValue(string key)
        {
            switch (key)
            {
                case "uint.5000":
                    return 5000u;

                case "float.pi":
                    return (float)Math.PI;

                default:
                    throw new KeyNotFoundException(key);
            }
        }
    }
}
