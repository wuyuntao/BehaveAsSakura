using System;

namespace BehaveAsSakura.Events
{
	public interface IEvent
	{
	}

	public interface IPublisher
	{

	}

	public interface ISubscriber
	{
		void OnEventTriggered(IEvent @event);
	}

	public sealed class EventBus
	{
		internal void Publish(IEvent @event)
		{
			throw new NotImplementedException();
		}

		internal void Subscribe<TEvent>(ISubscriber subscriber)
			where TEvent : IEvent
		{
			throw new NotImplementedException();
		}

		internal void Unsubscribe<TEvent>(ISubscriber subscriber)
			where TEvent : IEvent
		{
			throw new NotImplementedException();
		}
	}
}
