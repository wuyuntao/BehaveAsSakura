using BehaveAsSakura.Events;
using ProtoBuf;
using System.Collections.Generic;

namespace BehaveAsSakura.Timers
{
	[ProtoContract]
	class TimerManagerProps
	{
		[ProtoMember( 1 )]
		public uint MaxTimerId;

		[ProtoMember( 2 )]
		public TimerProps[] Timers;
	}

	class TimerManager
	{
		private BehaviorTree tree;
		private TimerManagerProps props;
		private List<Timer> timers = new List<Timer>();

		internal TimerManager(BehaviorTree tree)
		{
			this.tree = tree;
			this.props = new TimerManagerProps();
		}

		public Timer StartTimer(uint totalTime)
		{
			var timer = new Timer( tree, ++props.MaxTimerId, totalTime );
			timers.Add( timer );

			return timer;
		}

		public bool CancelTimer(Timer timer)
		{
			return timers.Remove( timer );
		}

		public void Update()
		{
			timers.RemoveAll( t =>
			{
				if( t.RemainingTime == 0 )
				{
					tree.Owner.EventBus.Publish( new TimerTriggeredEvent( t.Id ) );

					return true;
				}
				else
					return false;
			} );
		}
	}
}
