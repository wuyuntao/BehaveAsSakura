using BehaveAsSakura.Tasks;
using NUnit.Framework;

namespace BehaveAsSakura.Tests
{
	[TestFixture]
    class BehaviorTreeManagerTests
	{
        [Test]
        public void TestLogTree()
        {
            RunBehaviorTree("Log");
        }

        [Test]
		public void TestWaitTimerTree()
		{
			RunBehaviorTree( "WaitTimer" );
		}

		public static void RunBehaviorTree(string path)
        {
			var treeManagerOwner = new BehaviorTreeManagerOwner();
			var treeManager = new BehaviorTreeManager( treeManagerOwner );
			var treeOwner = new BehaviorTreeOwner();
            var tree = treeManager.CreateTree( treeOwner, path, null);

            while (tree.RootTask.LastResult == TaskResult.Running)
            {
                tree.Update();

				treeOwner.Tick( 100 );
            }
        }
	}
}
