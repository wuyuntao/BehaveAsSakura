using BehaveAsSakura.Utils;
using ProtoBuf;

namespace BehaveAsSakura.Timers
{
    [ProtoContract]
    class TimerProps
    {
        [ProtoMember(1)]
        public uint Id { get; set; }

        [ProtoMember(2)]
        public uint TotalTime { get; set; }

        [ProtoMember(3)]
        public uint EndTime { get; set; }
    }

    public sealed class Timer
    {
        private BehaviorTree tree;
        private TimerProps props;

        internal Timer(BehaviorTree tree, uint id, uint totalTime)
        {
            this.tree = tree;
            this.props = new TimerProps()
            {
                Id = id,
                TotalTime = totalTime,
                EndTime = tree.Owner.CurrentTime.SafeAdd(totalTime),
            };
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

        public uint Id
        {
            get { return props.Id; }
        }
    }
}
