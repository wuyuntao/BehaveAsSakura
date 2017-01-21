using BehaveAsSakura.Attributes;
using BehaveAsSakura.Utils;
using BehaveAsSakura.Variables;
using System;

namespace BehaveAsSakura.Tasks
{
    [BehaveAsTable]
    [BehaveAsUnionInclude(typeof(ITaskDesc), 4)]
    public sealed class LogTaskDesc : ILeafTaskDesc
    {
        [BehaveAsField(1)]
        public string Message { get; set; }

        [BehaveAsField(2, IsRequired = false)]
        public VariableDesc[] MessageParameters { get; set; }

        void ITaskDesc.Validate()
        {
            Message.ValidateNotEmpty(nameof(Message));

            if (MessageParameters != null)
            {
                foreach (var mp in MessageParameters)
                    mp.Validate();
            }
        }

        Task ITaskDesc.CreateTask(BehaviorTree tree, Task parentTask, uint id)
        {
            return new LogTask(tree, parentTask, id, this);
        }
    }

    class LogTask : LeafTask
    {
        private LogTaskDesc description;
        private Variable[] variables;

        public LogTask(BehaviorTree tree, Task parentTask, uint id, LogTaskDesc description)
            : base(tree, parentTask, id, description)
        {
            this.description = description;

            if (description.MessageParameters != null)
                variables = Array.ConvertAll(description.MessageParameters, desc => new Variable(desc));
        }

        protected override TaskResult OnUpdate()
        {
            if (variables != null)
                LogInfo(description.Message, Array.ConvertAll(variables, v => v.GetValue(this)));
            else
                LogInfo(description.Message);

            return TaskResult.Success;
        }
    }
}
