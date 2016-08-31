using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class SequenceTaskDesc : CompositeTaskDesc
	{
	}

	[ProtoContract]
	public class SequenceTaskProps : CompositeTaskProps
	{
		[ProtoMember( 1 )]
		public int CurrentChildIndex;

		public SequenceTaskProps(uint id)
			: base( id )
		{ }
	}

	public class SequenceTask : CompositeTask
	{
		private SequenceTaskProps props;

		public SequenceTask(BehaviorTree tree, Task parent, SequenceTaskDesc description)
			: this( tree, parent, description, new SequenceTaskProps( description.Id ) )
		{
			props = (SequenceTaskProps)Props;
		}

		protected SequenceTask(BehaviorTree tree, Task parent, SequenceTaskDesc description, SequenceTaskProps props)
			: base( tree, parent, description, props )
		{
			props = (SequenceTaskProps)Props;
		}

		protected override void OnStart()
		{
			base.OnStart();

			props.CurrentChildIndex = 0;
			GetChildTask( props.CurrentChildIndex ).EnqueueForUpdate();
		}

		protected override TaskResult OnUpdate()
		{
			return IterateChildTasks( TaskResult.Success );
		}

		protected TaskResult IterateChildTasks(TaskResult expectingResult)
		{
			var child = GetChildTask( props.CurrentChildIndex );
			if( child.LastResult == expectingResult )
			{
				if( ++props.CurrentChildIndex < ChildTaskCount )
				{
					GetChildTask( props.CurrentChildIndex ).EnqueueForUpdate();

					return TaskResult.Running;
				}
			}

			return child.LastResult;
		}
	}
}
