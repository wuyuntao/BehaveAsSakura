using BehaveAsSakura.Attributes;
using BehaveAsSakura.Events;
using BehaveAsSakura.Tasks;
using BehaveAsSakura.Timers;
using BehaveAsSakura.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaveAsSakura
{
    public interface ILogger
    {
        void LogDebug(string msg, params object[] args);

        void LogInfo(string msg, params object[] args);

        void LogWarning(string msg, params object[] args);

        void LogError(string msg, params object[] args);
    }

    interface ISerializable<T>
    {
        T CreateSnapshot();

        void RestoreSnapshot(T snapshot);
    }

    public interface IBehaviorTreeOwner : ILogger, IVariableContainer
    {
        uint CurrentTime { get; }
    }

    [BehaveAsTable]
    public class BehaviorTreeDesc
    {
        [BehaveAsField(1)]
        public TaskDescWrapper[] Tasks { get; set; }

        [BehaveAsField(2)]
        public uint RootTaskId { get; set; }

        internal TaskDescWrapper FindTaskDesc(uint id)
        {
            return Array.Find(Tasks, t => t.Id == id);
        }
    }

    [BehaveAsTable]
    public class BehaviorTreeProps
    {
        [BehaveAsField(1)]
        public EventBusProps EventBus { get; set; }

        [BehaveAsField(2)]
        public TimerManagerProps TimerManager { get; set; }

        [BehaveAsField(3)]
        public TaskPropsWrapper[] Tasks { get; set; }
    }

    public sealed class BehaviorTree : ISerializable<BehaviorTreeProps>
    {
        private BehaviorTreeManager treeManager;
        private IBehaviorTreeOwner owner;
        private Task parentTask;
        private Task rootTask;
        private Dictionary<uint, Task> tasks = new Dictionary<uint, Task>();
        private EventBus eventBus;
        private TimerManager timerManager;
        private TaskScheduler taskScheduler;

        internal BehaviorTree(BehaviorTreeManager treeManager, IBehaviorTreeOwner owner, BehaviorTreeDesc description, Task parentTask)
        {
            this.treeManager = treeManager;
            this.owner = owner;
            this.parentTask = parentTask;

            eventBus = new EventBus(this);
            timerManager = new TimerManager(this);
            taskScheduler = new TaskScheduler(this);

            rootTask = treeManager.CreateTask(this, description, parentTask, description.RootTaskId);
        }

        public override string ToString()
        {
            if (parentTask != null)
                return string.Format("{0} - {1}", parentTask, GetType().Name);
            else
                return string.Format("{0} - {1}", owner, GetType().Name);
        }

        #region Life-cycle

        public void Update()
        {
            eventBus.Update();
            timerManager.Update();
            taskScheduler.Update();
        }

        public void Abort()
        {
            rootTask.EnqueueForAbort();

            taskScheduler.Update();
        }

        #endregion

        #region Task Manipulation

        internal void InitializeTask(Task task)
        {
            tasks.Add(task.Id, task);
        }

        internal void EnqueueTask(Task task)
        {
            taskScheduler.Enqueue(task);
        }

        internal Task FindTask(uint id)
        {
            Task task;
            tasks.TryGetValue(id, out task);
            return task;
        }

        #endregion

        #region ISerializable

        public BehaviorTreeProps CreateSnapshot()
        {
            return ((ISerializable<BehaviorTreeProps>)this).CreateSnapshot();
        }

        public void RestoreSnapshot(BehaviorTreeProps snapshot)
        {
            ((ISerializable<BehaviorTreeProps>)this).RestoreSnapshot(snapshot);
        }

        BehaviorTreeProps ISerializable<BehaviorTreeProps>.CreateSnapshot()
        {
            var props = new BehaviorTreeProps()
            {
                EventBus = ((ISerializable<EventBusProps>)eventBus).CreateSnapshot(),
                TimerManager = ((ISerializable<TimerManagerProps>)timerManager).CreateSnapshot(),

                Tasks = (from t in tasks.Values
                         select ((ISerializable<TaskPropsWrapper>)t).CreateSnapshot()).ToArray(),

            };

            return props;
        }

        void ISerializable<BehaviorTreeProps>.RestoreSnapshot(BehaviorTreeProps snapshot)
        {
            ((ISerializable<EventBusProps>)eventBus).RestoreSnapshot(snapshot.EventBus);

            ((ISerializable<TimerManagerProps>)timerManager).RestoreSnapshot(snapshot.TimerManager);

            foreach (var ts in snapshot.Tasks)
            {
                var task = FindTask(ts.Id);
                if (task == null)
                    throw new InvalidOperationException(string.Format("Task #{0} does not exist"));

                ((ISerializable<TaskPropsWrapper>)task).RestoreSnapshot(ts);
            }
        }

        #endregion

        #region Properties

        public BehaviorTreeManager TreeManager
        {
            get { return treeManager; }
        }

        public IBehaviorTreeOwner Owner
        {
            get { return owner; }
        }

        public Task ParentTask
        {
            get { return parentTask; }
        }

        public Task RootTask
        {
            get { return rootTask; }
        }

        internal EventBus EventBus
        {
            get { return eventBus; }
        }

        internal TimerManager TimerManager
        {
            get { return timerManager; }
        }

        #endregion
    }
}
