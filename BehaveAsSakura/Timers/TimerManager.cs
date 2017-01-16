using BehaveAsSakura.Events;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;

namespace BehaveAsSakura.Timers
{
    [ProtoContract]
    class TimerManagerProps
    {
        [ProtoMember(1)]
        public uint MaxTimerId { get; set; }

        [ProtoMember(2)]
        public TimerProps[] Timers { get; set; }
    }

    class TimerManager : ISerializable<TimerManagerProps>
    {
        private BehaviorTree tree;
        private TimerManagerProps props;
        private List<Timer> timers = new List<Timer>();

        internal TimerManager(BehaviorTree tree)
        {
            this.tree = tree;
            this.props = new TimerManagerProps();
        }

        internal void Update()
        {
            timers.RemoveAll(t =>
           {
               if (t.RemainingTime == 0)
               {
                   tree.EventBus.Publish(new TimerTriggeredEvent(t.Id));
                   tree.Owner.LogDebug("[{0}] triggered", t);

                   return true;
               }
               else
               {
                   tree.Owner.LogDebug("[{0}] remains {1}", t, t.RemainingTime);
                   return false;
               }
           });
        }

        #region Timer Manipulation

        public Timer StartTimer(uint totalTime)
        {
            var timer = new Timer(tree, ++props.MaxTimerId, totalTime);
            timers.Add(timer);

            return timer;
        }

        public bool CancelTimer(Timer timer)
        {
            return timers.Remove(timer);
        }

        public Timer FindTimer(uint id)
        {
            return timers.Find(t => t.Id == id);
        }

        #endregion

        #region ISerializable

        TimerManagerProps ISerializable<TimerManagerProps>.CreateSnapshot()
        {
            return new TimerManagerProps()
            {
                MaxTimerId = props.MaxTimerId,
                Timers = (from t in timers
                          select ((ISerializable<TimerProps>)t).CreateSnapshot()).ToArray(),
            };
        }

        void ISerializable<TimerManagerProps>.RestoreSnapshot(TimerManagerProps snapshot)
        {
            props.MaxTimerId = snapshot.MaxTimerId;

            if (snapshot.Timers == null)
            {
                timers.Clear();
            }
            else
            {
                timers = new List<Timer>(from t in snapshot.Timers
                                         select RestoreOrCreateTimer(t));
            }
        }

        Timer RestoreOrCreateTimer(TimerProps props)
        {
            var timer = FindTimer(props.Id);
            if (timer != null)
            {
                ((ISerializable<TimerProps>)timer).RestoreSnapshot(props);

                return timer;
            }
            else
            {
                return new Timer(tree, props);
            }
        }

        #endregion
    }
}
