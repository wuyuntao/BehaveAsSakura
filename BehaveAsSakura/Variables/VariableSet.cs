using BehaveAsSakura.Attributes;
using System.Collections.Generic;

namespace BehaveAsSakura.Variables
{
    [BehaveAsTable]
    public class VariableSetProps
    {
        [BehaveAsField(1, IsRequired = false)]
        public NamedVariableDesc[] Variables { get; set; }
    }

    [BehaveAsTable]
    public class NamedVariableDesc
    {
        [BehaveAsField(1)]
        public string Name { get; set; }

        [BehaveAsField(2)]
        public VariableDesc Desc { get; set; }
    }

    public sealed class VariableSet : ISerializable<VariableSetProps>
    {
        private SortedList<string, Variable> variables = new SortedList<string, Variable>();

        public Variable Get(string key)
        {
            Variable variable;
            variables.TryGetValue(key, out variable);
            return variable;
        }

        public Variable Set(string key, VariableType type, VariableSource source, string value)
        {
            return Set(key, new VariableDesc(type, source, value));
        }

        public Variable Set(string key, VariableDesc description)
        {
            var variable = new Variable(description);
            variables[key] = variable;
            return variable;
        }

        public Variable Remove(string key)
        {
            var variable = Get(key);
            if (variable != null)
                variables.Remove(key);
            return variable;
        }

        public void RemoveAll()
        {
            variables.Clear();
        }

        #region ISerializable

        VariableSetProps ISerializable<VariableSetProps>.CreateSnapshot()
        {
            if (variables.Count == 0)
                return null;

            var descriptions = new List<NamedVariableDesc>();

            foreach (var variable in variables)
                descriptions.Add(new NamedVariableDesc() { Name = variable.Key, Desc = variable.Value.Description });

            return new VariableSetProps() { Variables = descriptions.ToArray() };
        }

        void ISerializable<VariableSetProps>.RestoreSnapshot(VariableSetProps snapshot)
        {
            variables.Clear();

            if (snapshot.Variables != null)
            {
                foreach (var variable in snapshot.Variables)
                    variables.Add(variable.Name, new Variable(variable.Desc));
            }
        }

        #endregion
    }
}
