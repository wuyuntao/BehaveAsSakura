using System;
using System.Collections.Generic;

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
        void OnEventTriggered(IPublisher publisher, IEvent @event);
    }

    public sealed class EventBus
    {
        private Dictionary<Type, Subscription> subscriptions = new Dictionary<Type, Subscription>();

        public void Publish(IPublisher publisher, IEvent @event)
        {
            var subscription = GetSubscription(@event.GetType(), false);
            if (subscription != null)
            {
                foreach (var subscriber in subscription.Subscribers)
                    subscriber.OnEventTriggered(publisher, @event);
            }
        }

        public void Subscribe<TEvent>(ISubscriber subscriber)
            where TEvent : IEvent
        {
            var subscription = GetSubscription(typeof(TEvent), true);

            subscription.AddSubscriber(subscriber);
        }

        public void Unsubscribe<TEvent>(ISubscriber subscriber)
            where TEvent : IEvent
        {
            var subscription = GetSubscription(typeof(TEvent), false);
            if (subscription != null)
                subscription.RemoveSubscriber(subscriber);
        }

        Subscription GetSubscription(Type type, bool createIfNotExist)
        {
            Subscription subscription;
            if (!subscriptions.TryGetValue(type, out subscription))
            {
                subscription = new Subscription();
                subscriptions.Add(type, subscription);
            }
            return subscription;
        }

        class Subscription
        {
            private List<ISubscriber> subscribers = new List<ISubscriber>();

            public void AddSubscriber(ISubscriber subscriber)
            {
                if (!subscribers.Contains(subscriber))
                    subscribers.Add(subscriber);
            }

            public void RemoveSubscriber(ISubscriber subscriber)
            {
                subscribers.Remove(subscriber);
            }

            public IEnumerable<ISubscriber> Subscribers
            {
                get { return subscribers; }
            }
        }
    }
}