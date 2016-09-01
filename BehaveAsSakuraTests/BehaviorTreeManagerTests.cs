using BehaveAsSakura.Events;
using BehaveAsSakura.Tasks;
using BehaveAsSakura.Variables;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace BehaveAsSakura.Tests
{
    [TestFixture]
    class BehaviorTreeManagerTests : IBehaviorTreeManagerOwner
	{
        [Test]
        public void TestLogTree()
        {
            RunBehaviorTree("Log");
            RunBehaviorTree("WaitTimer");
        }

        void RunBehaviorTree(string path)
        {
			var owner = new BehaviorTreeOwner();
			var treeManager = new BehaviorTreeManager(this);
            var tree = treeManager.CreateTree( owner, path, null);

            while (tree.RootTask.LastResult == TaskResult.Running)
            {
                tree.Update();

				owner.Tick( 100 );
            }
        }

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
                    throw new KeyNotFoundException(path);
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
