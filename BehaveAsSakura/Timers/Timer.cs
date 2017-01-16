using BehaveAsSakura.Attributes;
using BehaveAsSakura.Utils;

namespace BehaveAsSakura.Timers
{
    [BehaveAsContract]
    class TimerProps
    {
        [BehaveAsMember(1)]
        public uint Id { get; set; }

        [BehaveAsMember(2)]
        public uint TotalTime { get; set; }

        [BehaveAsMember(3)]
        public uint EndTime { get; set; }
    }

    public sealed class Timer : ISerializable<TimerProps>
    {
        private BehaviorTree tree;
        private TimerProps props;

        internal Timer(BehaviorTree tree, uint id, uint totalTime)
            : this(tree, new TimerProps()
            {
                Id = id,
                TotalTime = totalTime,
                EndTime = tree.Owner.CurrentTime.SafeAdd(totalTime),
            })
        { }

        internal Timer(BehaviorTree tree, TimerProps props)
        {
            this.tree = tree;
            this.props = props;
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}(#{2})", tree, GetType().Name, props.Id);
        }

        #region ISerializable

        TimerProps ISerializable<TimerProps>.CreateSnapshot()
        {
            return props;
        }

        void ISerializable<TimerProps>.RestoreSnapshot(TimerProps snapshot)
        {
            props = snapshot;
        }

        #endregion

        #region Properties

        public uint Id
        {
            get { return props.Id; }
        }

        public uint TotalTime
        {
            get { return props.TotalTime; }
        }

        public uint ElapsedTime
        {
            get { return TotalTime.SafeSub(RemainingTime); }
        }

        public uint RemainingTime
        {
            get { return props.EndTime.SafeSub(tree.Owner.CurrentTime); }
        }

        #endregion
    }
}
