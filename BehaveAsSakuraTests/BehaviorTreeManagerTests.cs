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
    class BehaviorTreeManagerTests : IBehaviorTreeLoader, IBehaviorTreeOwner
    {
        private Stopwatch timer = Stopwatch.StartNew();
        private EventBus eventBus = new EventBus();

        [Test]
        public void TestLogTree()
        {
            RunBehaviorTree("Log");
            RunBehaviorTree("WaitTimer");
        }

        void RunBehaviorTree(string path)
        {
            var treeManager = new BehaviorTreeManager(this);
            var tree = treeManager.CreateTree(this, path, null);

            timer.Restart();
            int i = 0;
            while (tree.RootTask.LastResult == TaskResult.Running)
            {
                tree.Update();

                Console.WriteLine("#{0} Tree updated: {1}", ++i, tree.RootTask.LastResult);
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

                case "WaitTimer":
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

                default:
                    throw new KeyNotFoundException(path);
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
