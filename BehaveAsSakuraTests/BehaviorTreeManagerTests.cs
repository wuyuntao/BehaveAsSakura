using BehaveAsSakura.Events;
using BehaveAsSakura.Tasks;
using BehaveAsSakura.Variables;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;

namespace BehaveAsSakura.Tests
{
    [TestFixture]
    class BehaviorTreeManagerTests : IBehaviorTreeLoader, IBehaviorTreeOwner
    {
        private Stopwatch timer = Stopwatch.StartNew();
        private EventBus eventBus = new EventBus();

        [Test]
        public void TestLogTree()
        {
            var treeManager = new BehaviorTreeManager(this);
            var tree = treeManager.CreateTree(this, "Log", null);

            while (tree.RootTask.LastResult == TaskResult.Running)
            {
                tree.Update();

                Console.WriteLine("Tree updated: {0}", tree.RootTask.LastResult);
                Thread.Sleep(100);
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

                default:
                    {
                        builder.Composite<SequenceTaskDesc>()
                            .AppendChild(builder.Leaf<LogTaskDesc>(
                                    d => d.Message = "Start"))
                            .AppendChild(builder.Leaf<WaitTimerTaskDesc>(
                                    d => d.Time = new VariableDesc(VariableType.UInteger, VariableSource.LiteralConstant, "3000")))
                            .AppendChild(builder.Leaf<LogTaskDesc>(d =>
                                  d.Message = "End"));
                        break;
                    }
            }

            return builder.Build();
        }

        void ILogger.LogDebug(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

        void ILogger.LogError(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

        void ILogger.LogInfo(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

        void ILogger.LogWarning(string msg, params object[] args)
        {
            Console.WriteLine(msg, args);
        }

        uint IBehaviorTreeOwner.CurrentTime
        {
            get { return (uint)timer.ElapsedMilliseconds; }
        }

        EventBus IBehaviorTreeOwner.EventBus
        {
            get { return eventBus; }
        }
    }
}
