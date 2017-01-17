using BehaveAsSakura.Attributes;
using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaveAsSakura.Events
{
    public interface IEvent
    {
    }

    [BehaveAsTable]
    public class SubscriptionProps
    {
        [BehaveAsField(1)]
        public string Type { get; set; }

        [BehaveAsField(2, IsRequired = false)]
        public uint[] TaskIds { get; set; }

        public SubscriptionProps(string type, IEnumerable<Task> subscribers)
        {
            Type = type;
            TaskIds = (from t in subscribers
                       select t.Id).ToArray();
        }

        public SubscriptionProps()
        { }
    }

    [BehaveAsTable]
    public class EventBusProps
    {
        [BehaveAsField(1, IsRequired = false)]
        public IEvent[] Events { get; set; }

        [BehaveAsField(2)]
        public SubscriptionProps[] Subscriptions { get; set; }
    }

    class EventBus : ISerializable<EventBusProps>
    {
        private BehaviorTree tree;
        private List<IEvent> events = new List<IEvent>();
        private Dictionary<string, Subscription> subscriptions = new Dictionary<string, Subscription>();

        internal EventBus(BehaviorTree tree)
        {
            this.tree = tree;
        }

        internal void Update()
        {
            foreach (var @event in events)
            {
                var subscription = GetSubscription(@event.GetType().FullName, false);
                if (subscription != null)
                {
                    var subscribers = subscription.Subscribers.ToArray();
                    foreach (var subscriber in subscribers)
                        subscriber.OnEventTriggered(@event);
                }
            }
        }

        public void Publish(IEvent @event)
        {
            events.Add(@event);
        }

        public void Subscribe<TEvent>(Task subscriber)
            where TEvent : IEvent
        {
            var subscription = GetSubscription(typeof(TEvent).FullName, true);

            subscription.AddSubscriber(subscriber);
        }

        public void Unsubscribe<TEvent>(Task subscriber)
            where TEvent : IEvent
        {
            var subscription = GetSubscription(typeof(TEvent).FullName, false);
            if (subscription != null)
                subscription.RemoveSubscriber(subscriber);
        }

        Subscription GetSubscription(string type, bool createIfNotExist)
        {
            Subscription subscription;
            if (!subscriptions.TryGetValue(type, out subscription))
            {
                subscription = new Subscription();
                subscriptions.Add(type, subscription);
            }
            return subscription;
        }

        #region ISerializable

        EventBusProps ISerializable<EventBusProps>.CreateSnapshot()
        {
            return new EventBusProps()
            {
                Events = events.ToArray(),

                Subscriptions = (from s in subscriptions
                                 select new SubscriptionProps(s.Key, s.Value.Subscribers)).ToArray(),
            };
        }

        void ISerializable<EventBusProps>.RestoreSnapshot(EventBusProps snapshot)
        {
            events.Clear();

            if (snapshot.Events != null)
            {
                events.AddRange(snapshot.Events);
            }

            subscriptions.Clear();

            if (snapshot.Subscriptions != null)
            {
                foreach (var s in snapshot.Subscriptions)
                {
                    var subscription = GetSubscription(s.Type, true);
                    foreach (var t in s.TaskIds)
                    {
                        var task = tree.FindTask(t);
                        if (task == null)
                            throw new InvalidOperationException("Failed to find task");

                        subscription.AddSubscriber(task);
                    }
                }
            }
        }

        #endregion

        #region Subscription

        class Subscription
        {
            private List<Task> subscribers = new List<Task>();

            public void AddSubscriber(Task subscriber)
            {
                if (!subscribers.Contains(subscriber))
                    subscribers.Add(subscriber);
            }

            public void RemoveSubscriber(Task subscriber)
            {
                subscribers.Remove(subscriber);
            }

            public IEnumerable<Task> Subscribers
            {
                get { return subscribers; }
            }
        }

        #endregion
    }
}