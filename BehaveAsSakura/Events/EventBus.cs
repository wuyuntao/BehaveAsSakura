using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaveAsSakura.Events
{
	public interface IEvent
	{
	}

	public interface ISubscriber
	{
		void OnEventTriggered(IEvent @event);
	}

	[ProtoContract]
	class EventBusProps
	{
		[ProtoMember( 1, IsRequired = false, DynamicType = true )]
		public object[] Events { get; set; }
	}

	public sealed class EventBus : ISerializable<EventBusProps>
	{
		private List<IEvent> events = new List<IEvent>();
		private Dictionary<Type, Subscription> subscriptions = new Dictionary<Type, Subscription>();

		internal EventBus()
		{ }

		internal void Update()
		{
			foreach( var @event in events )
			{
				var subscription = GetSubscription( @event.GetType(), false );
				if( subscription != null )
				{
					var subscribers = subscription.Subscribers.ToArray();
					foreach( var subscriber in subscribers )
						subscriber.OnEventTriggered( @event );
				}
			}
		}

		public void Publish(IEvent @event)
		{
			events.Add( @event );
		}

		public void Subscribe<TEvent>(ISubscriber subscriber)
			where TEvent : IEvent
		{
			var subscription = GetSubscription( typeof( TEvent ), true );

			subscription.AddSubscriber( subscriber );
		}

		public void Unsubscribe<TEvent>(ISubscriber subscriber)
			where TEvent : IEvent
		{
			var subscription = GetSubscription( typeof( TEvent ), false );
			if( subscription != null )
				subscription.RemoveSubscriber( subscriber );
		}

		Subscription GetSubscription(Type type, bool createIfNotExist)
		{
			Subscription subscription;
			if( !subscriptions.TryGetValue( type, out subscription ) )
			{
				subscription = new Subscription();
				subscriptions.Add( type, subscription );
			}
			return subscription;
		}

		#region ISerializable

		EventBusProps ISerializable<EventBusProps>.CreateSnapshot()
		{
			return new EventBusProps() { Events = events.ToArray() };
		}

		void ISerializable<EventBusProps>.RestoreSnapshot(EventBusProps snapshot)
		{
			subscriptions.Clear();

			if( snapshot.Events != null )
			{
				events = new List<IEvent>( from e in snapshot.Events
										   select (IEvent)e );
			}
			else
			{
				events.Clear();
			}
		}

		#endregion

		#region Subscription

		class Subscription
		{
			private List<ISubscriber> subscribers = new List<ISubscriber>();

			public void AddSubscriber(ISubscriber subscriber)
			{
				if( !subscribers.Contains( subscriber ) )
					subscribers.Add( subscriber );
			}

			public void RemoveSubscriber(ISubscriber subscriber)
			{
				subscribers.Remove( subscriber );
			}

			public IEnumerable<ISubscriber> Subscribers
			{
				get { return subscribers; }
			}
		}

		#endregion
	}
}