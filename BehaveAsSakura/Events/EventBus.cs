using BehaveAsSakura.Tasks;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaveAsSakura.Events
{
    public interface IEvent
    {
    }

    [ProtoContract]
    class SubscriptionProps
    {
        [ProtoMember(1)]
        public Type Type { get; set; }

        [ProtoMember(2)]
        public uint[] TaskIds { get; set; }

        public SubscriptionProps(Type type, IEnumerable<Task> subscribers)
        {
            Type = type;
            TaskIds = (from t in subscribers
                       select t.Id).ToArray();
        }

        public SubscriptionProps()
        { }
    }

    [ProtoContract]
    class EventBusProps
    {
        [ProtoMember(1, IsRequired = false, DynamicType = true)]
        public object[] Events { get; set; }

        [ProtoMember(2)]
        public SubscriptionProps[] Subscriptions { get; set; }
    }

    class EventBus : ISerializable<EventBusProps>
    {
        private BehaviorTree tree;
        private List<IEvent> events = new List<IEvent>();
        private Dictionary<Type, Subscription> subscriptions = new Dictionary<Type, Subscription>();

        internal EventBus(BehaviorTree tree)
        {
            this.tree = tree;
        }

        internal void Update()
        {
            foreach (var @event in events)
            {
                var subscription = GetSubscription(@event.GetType(), false);
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
            var subscription = GetSubscription(typeof(TEvent), true);

            subscription.AddSubscriber(subscriber);
        }

        public void Unsubscribe<TEvent>(Task subscriber)
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
                events.AddRange(from e in snapshot.Events
                                select (IEvent)e);
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