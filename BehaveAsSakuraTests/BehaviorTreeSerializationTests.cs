using BehaveAsSakura.Tasks;
using NUnit.Framework;
using ProtoBuf;
using System;
using System.IO;

namespace BehaveAsSakura.Tests
{
	[TestFixture]
	class BehaviorTreeSerializationTests
	{
		[Test]
		public void TestRestore()
		{
			var treeManagerOwner = new BehaviorTreeManagerOwner();
			var treeManager = new BehaviorTreeManager( treeManagerOwner );
			var treeOwner = new BehaviorTreeOwner();
			var tree1 = treeManager.CreateTree( treeOwner, "WaitTimer", null );

			Console.WriteLine( "============ Tree1 ============" );

			for( int i = 0; i < 20; i++ )
			{
				tree1.Update();
				treeOwner.Tick( 100 );
			}

			var snapshot1 = tree1.CreateSnapshot();

			BehaviorTreeProps snapshot2;
			using( var stream1 = new MemoryStream() )
			{
				Serializer.Serialize( stream1, snapshot1 );

				using( var stream2 = new MemoryStream( stream1.ToArray() ) )
				{
					snapshot2 = Serializer.Deserialize<BehaviorTreeProps>( stream2 );
				}
			}

			Console.WriteLine( "============ Tree2 ============" );

			var tree2 = treeManager.CreateTree( treeOwner, "WaitTimer", null );

			//for( int i = 0; i < 30; i++ )
			//{
			//	tree2.Update();
			//	treeOwner.Tick( 100 );
			//}

			Console.WriteLine( "============ Tree2 Restore ============" );

			tree2.RestoreSnapshot( snapshot2 );

			for( int i = 0; i < 1000 && tree2.RootTask.LastResult == TaskResult.Running; i++ )
			{
				tree2.Update();

				treeOwner.Tick( 100 );
			}
		}
	}
}
