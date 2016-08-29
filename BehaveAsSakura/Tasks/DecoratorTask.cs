using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaveAsSakura.Tasks
{
    public abstract class DecoratorTask : Task
    {
        protected DecoratorTask(BehaviorTree tree, Task parent, TaskDesc description, TaskProps props)
            : base(tree, parent, description, props)
        { }
    }
}
