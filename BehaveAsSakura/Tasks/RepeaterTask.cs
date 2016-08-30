using ProtoBuf;

namespace BehaveAsSakura.Tasks
{
	[ProtoContract]
	public class RepeaterTaskDesc : DecoratorTaskDesc
	{
		[ProtoMember( 1 )]
		public uint Count { get; set; }
	}

	[ProtoContract]
	public class RepeaterTaskProps : DecoratorTaskProps
	{
		[ProtoMember( 1 )]
		public bool WaitForChildCompleted;

		[ProtoMember( 2 )]
		public uint LastUpdateTime;

		[ProtoMember( 3 )]
		public uint Count;

		public RepeaterTaskProps(uint id)
			: base( id )
		{ }
	}

	public class RepeaterTask : DecoratorTask
	{
		private RepeaterTaskDesc description;
		private RepeaterTaskProps props;

		public RepeaterTask(BehaviorTree tree, Task parent, RepeaterTaskDesc description)
			: this( tree, parent, description, new RepeaterTaskProps( description.Id ) )
		{
		}

		protected RepeaterTask(BehaviorTree tree, Task parent, RepeaterTaskDesc description, RepeaterTaskProps props)
			: base( tree, parent, description, props )
		{
			this.description = description;
			this.props = props;
		}

		protected override void OnStart()
		{
			base.OnStart();

			props.WaitForChildCompleted = true;
			props.Count = 0;

			ChildTask.EnqueueForUpdate();
		}

		protected override TaskResult OnUpdate()
		{
			if( props.WaitForChildCompleted )
			{
				if( ChildTask.LastResult == TaskResult.Running )
					return TaskResult.Running;

				if( description.Count > 0 && ++props.Count >= description.Count )
					return TaskResult.Failure;

				if( !IsRepeaterCompleted( ChildTask.LastResult ) )
				{
					props.WaitForChildCompleted = false;
					if( Owner.CurrentTime > props.LastUpdateTime )
						EnqueueForUpdate();
					else
						EnqueueForNextUpdate();
				}
			}
			else
			{
				props.WaitForChildCompleted = true;
				ChildTask.EnqueueForUpdate();
			}

			props.LastUpdateTime = Owner.CurrentTime;

			return TaskResult.Running;
		}

		protected virtual bool IsRepeaterCompleted(TaskResult result)
		{
			return false;
		}
	}
}