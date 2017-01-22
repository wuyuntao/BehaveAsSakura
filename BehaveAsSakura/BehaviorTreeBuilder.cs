using BehaveAsSakura.Tasks;
using System;
using System.Collections.Generic;

namespace BehaveAsSakura
{
    public sealed class BehaviorTreeBuilder
    {
        private string title;
        private string comment;
        private uint maxTaskId;
        private List<TaskDescWrapper> tasks = new List<TaskDescWrapper>();

        public BehaviorTreeBuilder(string title = null, string comment = null)
        {
            this.title = title;
            this.comment = comment;
        }

        public CompositeTaskBuilder Composite<T>(string title, Action<T> initializer = null)
            where T : ICompositeTaskDesc, new()
        {
            var task = CreateTask<CompositeTaskDescWrapper, T>(title, initializer);

            return new CompositeTaskBuilder(task);
        }

        public CompositeTaskBuilder Composite<T>(Action<T> initializer = null)
            where T : ICompositeTaskDesc, new()
        {
            return Composite<T>(null, initializer);
        }

        public DecoratorTaskBuilder Decorator<T>(string title, Action<T> initializer = null)
            where T : IDecoratorTaskDesc, new()
        {
            var task = CreateTask<DecoratorTaskDescWrapper, T>(title, initializer);

            return new DecoratorTaskBuilder(task);
        }

        public DecoratorTaskBuilder Decorator<T>(Action<T> initializer = null)
            where T : IDecoratorTaskDesc, new()
        {
            return Decorator<T>(null, initializer);
        }

        public LeafTaskBuilder Leaf<T>(string title, Action<T> initializer = null)
            where T : ILeafTaskDesc, new()
        {
            var task = CreateTask<LeafTaskDescWrapper, T>(title, initializer);

            return new LeafTaskBuilder(task);
        }

        public LeafTaskBuilder Leaf<T>(Action<T> initializer = null)
            where T : ILeafTaskDesc, new()
        {
            return Leaf(null, initializer);
        }

        public BehaviorTreeDesc Build()
        {
            return new BehaviorTreeDesc()
            {
                Title = title,
                Comment = comment,
                Tasks = tasks.ToArray(),
                RootTaskId = 1,
            };
        }

        TTaskDescWrapper CreateTask<TTaskDescWrapper, TTaskDesc>(string title, Action<TTaskDesc> initializer)
            where TTaskDescWrapper : TaskDescWrapper, new()
            where TTaskDesc : ITaskDesc, new()
        {
            var id = ++maxTaskId;
            var desc = new TTaskDesc();
            var task = new TTaskDescWrapper()
            {
                Id = id,
                Title = title != null ? title : string.Format("{0}-{1}", typeof(TTaskDesc).Name, id),
                CustomDesc = desc,
            };

            initializer?.Invoke(desc);

            tasks.Add(task);

            return task;
        }
    }

    public abstract class TaskBuilder
    {
        private TaskDescWrapper task;

        internal TaskBuilder(TaskDescWrapper task)
        {
            this.task = task;
        }

        internal TaskDescWrapper Task
        {
            get { return task; }
        }
    }

    public sealed class CompositeTaskBuilder : TaskBuilder
    {
        private CompositeTaskDescWrapper task;

        internal CompositeTaskBuilder(CompositeTaskDescWrapper task)
            : base(task)
        {
            this.task = task;
        }

        public CompositeTaskBuilder AppendChild(TaskBuilder builder)
        {
            task.ChildTaskIds.Add(builder.Task.Id);

            return this;
        }
    }

    public sealed class DecoratorTaskBuilder : TaskBuilder
    {
        private DecoratorTaskDescWrapper task;

        internal DecoratorTaskBuilder(DecoratorTaskDescWrapper task)
            : base(task)
        {
            this.task = task;
        }

        public DecoratorTaskBuilder SetChild(TaskBuilder builder)
        {
            task.ChildTaskId = builder.Task.Id;

            return this;
        }
    }

    public sealed class LeafTaskBuilder : TaskBuilder
    {
        internal LeafTaskBuilder(LeafTaskDescWrapper task)
            : base(task)
        {
        }
    }
}
