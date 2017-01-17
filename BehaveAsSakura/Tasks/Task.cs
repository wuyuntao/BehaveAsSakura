using BehaveAsSakura.Attributes;
using BehaveAsSakura.Events;
using BehaveAsSakura.Timers;
using BehaveAsSakura.Variables;
using System;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsEnum]
    public enum TaskResult : byte
    {
        Running,
        Success,
        Failure,
    }

    [BehaveAsEnum]
    public enum TaskState : byte
    {
        Suspend,            // (WaitForUpdate, WaitForAbort) -> Suspend -> WaitForStart
        WaitForStart,       // Suspend -> WaitForStart -> (WaitForUpdate, WaitForAbort)
        WaitForUpdate,      // WaitForStart -> WaitForUpdate -> (WaitForEnqueue, WaitForAbort, Suspend)
        WaitForAbort,       // (WaitForStart, WaitForUpdate, WaitForEnqueue) -> WaitForAbort -> Suspend
        WaitForEnqueue,     // WaitForUpdate -> WaitForEnqueue -> (WaitForUpdate, WaitForAbort)
    }

    public interface ITaskDesc
    {
        Task CreateTask(BehaviorTree tree, Task parentTask, uint id);
    }

    public interface ITaskProps
    {
    }

    public abstract class TaskDescWrapper
    {
        [BehaveAsField(1)]
        public uint Id { get; set; }

        [BehaveAsField(2, IsRequired = false)]
        public string Name { get; set; }

        [BehaveAsField(3, IsRequired = false)]
        public string Comment { get; set; }

        [BehaveAsField(4, IsRequired = false)]
        public ITaskDesc CustomDesc { get; set; }
    }

    [BehaveAsTable]
    public class TaskPropsWrapper
    {
        [BehaveAsField(1)]
        public uint Id { get; set; }

        [BehaveAsField(2)]
        public TaskState State { get; set; }

        [BehaveAsField(3)]
        public TaskResult LastResult { get; set; }

        [BehaveAsField(4, IsRequired = false)]
        public VariableSetProps SharedVariables { get; set; }

        [BehaveAsField(5, IsRequired = false)]
        public ITaskProps CustomProps { get; set; }
    }

    public abstract class Task : ILogger, ISerializable<TaskPropsWrapper>
    {
        private BehaviorTree tree;
        private Task parentTask;
        private uint id;
        private ITaskDesc description;

        private TaskState state = TaskState.Suspend;
        private TaskResult lastResult = TaskResult.Running;
        private ITaskProps props;
        private Timer immediateTimer;
        private VariableSet sharedVariables;

        protected Task(BehaviorTree tree, Task parentTask, uint id, ITaskDesc description, ITaskProps props)
        {
            this.tree = tree;
            this.parentTask = parentTask;
            this.id = id;
            this.description = description;
            this.props = props;

            LogDebug("[{0}] is created", this);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}(#{2})", tree, GetType().Name, id);
        }

        #region Life-cycle

        internal void Update()
        {
            switch (state)
            {
                case TaskState.WaitForStart:
                    state = TaskState.WaitForUpdate;
                    OnStart();
                    Update();
                    break;

                case TaskState.WaitForUpdate:
                    state = TaskState.WaitForEnqueue;
                    var result = OnUpdate();
                    LogDebug("[{0}] updated with result {1}", this, result);

                    if (result != TaskResult.Running)
                    {
                        state = TaskState.Suspend;
                        lastResult = result;
                        OnEnd();

                        if (parentTask != null)
                            parentTask.EnqueueForUpdate();
                    }
                    break;


                case TaskState.WaitForAbort:
                    state = TaskState.Suspend;
                    lastResult = TaskResult.Failure;
                    OnAbort();
                    OnEnd();
                    break;

                default:
                    break;
            }
        }

        internal protected void EnqueueForUpdate()
        {
            switch (state)
            {
                case TaskState.Suspend:
                    lastResult = TaskResult.Running;
                    state = TaskState.WaitForStart;
                    tree.EnqueueTask(this);
                    break;

                case TaskState.WaitForEnqueue:
                    state = TaskState.WaitForUpdate;
                    tree.EnqueueTask(this);
                    break;

                default:
                    LogDebug("[{0}] ignored update request due to stage {1}", this, state);
                    break;
            }
        }

        internal protected void EnqueueForAbort()
        {
            switch (state)
            {
                case TaskState.WaitForStart:
                case TaskState.WaitForUpdate:
                case TaskState.WaitForEnqueue:
                    state = TaskState.WaitForAbort;
                    tree.EnqueueTask(this);
                    break;

                default:
                    LogDebug("[{0}] ignored abort request due to stage {1}", this, state);
                    break;
            }
        }

        internal protected void EnqueueForNextUpdate()
        {
            if (immediateTimer != null)
                tree.TimerManager.CancelTimer(immediateTimer);

            immediateTimer = tree.TimerManager.StartTimer(0);
            Tree.EventBus.Subscribe<TimerTriggeredEvent>(this);
        }

        protected virtual void OnStart()
        {
            LogDebug("[{0}] started", this);
        }

        protected abstract TaskResult OnUpdate();

        protected virtual void OnEnd()
        {
            LogDebug("[{0}] ended", this);

            if (sharedVariables != null)
                sharedVariables = null;

            Tree.EventBus.Unsubscribe<TimerTriggeredEvent>(this);
        }

        protected virtual void OnAbort()
        {
            LogDebug("[{0}] aborted", this);
        }

        #endregion

        #region Shapred Variable Methods

        internal protected Variable GetSharedVariable(string key)
        {
            Task task;
            return GetSharedVariable(key, out task);
        }

        internal protected Variable GetSharedVariable(string key, out Task task)
        {
            Variable variable;
            if (sharedVariables != null)
                variable = sharedVariables.Get(key);
            else
                variable = null;

            if (variable != null)  // Found in current task
                task = this;
            else if (ParentTask != null)   // Try find in ancestor nodes
                variable = ParentTask.GetSharedVariable(key, out task);
            else
                task = null;

            return variable;
        }

        internal protected Variable GetSharedVariableFromParent(string key)
        {
            Task task;
            return GetSharedVariableFromParent(key, out task);
        }

        internal protected Variable GetSharedVariableFromParent(string key, out Task task)
        {
            if (ParentTask == null)
                throw new InvalidOperationException("No parent");

            return ParentTask.GetSharedVariable(key, out task);
        }

        internal protected void SetSharedVariable(string key, VariableDesc variable)
        {
            if (sharedVariables == null)
                sharedVariables = new VariableSet();

            sharedVariables.Set(key, variable);
        }

        internal protected void SetSharedVariable(string key, VariableType type, VariableSource source, string value)
        {
            SetSharedVariable(key, new VariableDesc(type, source, value));
        }

        internal protected void SetSharedVariableToParent(string key, VariableDesc variable)
        {
            if (ParentTask == null)
                throw new InvalidOperationException("No parent");

            ParentTask.SetSharedVariable(key, variable);
        }

        internal protected void SetSharedVariableToParent(string key, VariableType type, VariableSource source, string value)
        {
            SetSharedVariableToParent(key, new VariableDesc(type, source, value));
        }

        #endregion

        #region Timer Manipulation

        protected Timer StartTimer(uint totalTime)
        {
            return tree.TimerManager.StartTimer(totalTime);
        }

        protected bool CancelTimer(Timer timer)
        {
            return tree.TimerManager.CancelTimer(timer);
        }

        protected Timer FindTimer(uint id)
        {
            return tree.TimerManager.FindTimer(id);
        }

        #endregion

        #region Event Manipulation

        protected void PublishEvent(IEvent @event)
        {
            tree.EventBus.Publish(@event);
        }

        protected void SubscribeEvent<TEvent>()
            where TEvent : IEvent
        {
            tree.EventBus.Subscribe<TEvent>(this);
        }

        protected void UnsubscribeEvent<TEvent>()
            where TEvent : IEvent
        {
            tree.EventBus.Unsubscribe<TEvent>(this);
        }

        internal protected virtual void OnEventTriggered(IEvent @event)
        {
            var timerTriggered = @event as TimerTriggeredEvent;
            if (timerTriggered != null && immediateTimer != null && immediateTimer.Id == timerTriggered.TimerId)
            {
                immediateTimer = null;
                Tree.EventBus.Unsubscribe<TimerTriggeredEvent>(this);
                EnqueueForUpdate();
            }
        }

        #endregion

        #region ILogger

        public void LogDebug(string msg, params object[] args)
        {
            tree.Owner.LogDebug(msg, args);
        }

        public void LogInfo(string msg, params object[] args)
        {
            tree.Owner.LogInfo(msg, args);
        }

        public void LogWarning(string msg, params object[] args)
        {
            tree.Owner.LogWarning(msg, args);
        }

        public void LogError(string msg, params object[] args)
        {
            tree.Owner.LogError(msg, args);
        }

        #endregion

        #region ISerializable

        TaskPropsWrapper ISerializable<TaskPropsWrapper>.CreateSnapshot()
        {
            return new TaskPropsWrapper()
            {
                Id = id,
                State = state,
                LastResult = lastResult,
                SharedVariables = sharedVariables != null ? ((ISerializable<VariableSetProps>)sharedVariables).CreateSnapshot() : null,
                CustomProps = OnCloneProps(),
            };
        }

        protected virtual ITaskProps OnCloneProps()
        {
            return props;
        }

        void ISerializable<TaskPropsWrapper>.RestoreSnapshot(TaskPropsWrapper snapshot)
        {
            if (snapshot.Id != id)
                throw new InvalidOperationException("id of snapshot does not match");

            state = snapshot.State;
            lastResult = snapshot.LastResult;

            if (snapshot.SharedVariables != null)
            {
                sharedVariables = new VariableSet();
                ((ISerializable<VariableSetProps>)sharedVariables).RestoreSnapshot(snapshot.SharedVariables);
            }
            else
                sharedVariables = null;

            OnRestoreProps((ITaskProps)snapshot.CustomProps);
        }

        protected virtual void OnRestoreProps(ITaskProps props)
        {
            this.props = props;
        }

        #endregion

        #region Properties

        internal protected BehaviorTree Tree
        {
            get { return tree; }
        }

        internal protected IBehaviorTreeOwner Owner
        {
            get { return tree.Owner; }
        }

        internal protected Task ParentTask
        {
            get
            {
                if (parentTask != null)
                    return parentTask;

                if (tree.ParentTask != null)
                    return tree.ParentTask;

                return null;
            }
        }

        internal protected uint Id
        {
            get { return id; }
        }

        internal protected ITaskDesc Description
        {
            get { return description; }
        }

        protected ITaskProps Props
        {
            get { return props; }
        }

        public TaskResult LastResult
        {
            get { return lastResult; }
        }

        #endregion
    }
}
