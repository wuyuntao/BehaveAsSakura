using BehaveAsSakura.Tasks;
using NUnit.Framework;

namespace BehaveAsSakura.Tests
{
    [TestFixture]
    class ConditionEvaluatorTaskTests
    {
        [Test]
        public void TestInvalidVariable()
        {
            var treeManagerOwner = new BehaviorTreeManagerOwner();
            var treeManager = new BehaviorTreeManager(treeManagerOwner);
            var treeOwner = new BehaviorTreeOwner();
            var tree = treeManager.CreateTree(treeOwner, "ConditionEvaluator1", null);

            while (tree.RootTask.LastResult == TaskResult.Running)
            {
                tree.Update();
                treeOwner.Tick(100);
            }
        }
    }
}
