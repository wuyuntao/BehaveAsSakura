using BehaveAsSakura.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BehaveAsSakura.SerializationCompiler.Schema
{
    class SchemaDef
    {
        public string Namespace { get; private set; }

        public List<EnumDef> Enums { get; private set; }

        public List<TableDef> Tables { get; private set; }

        public List<UnionDef> Unions { get; private set; }

        public SchemaDef(string ns, IEnumerable<Assembly> assemblies)
        {
            Namespace = ns;

            SearchEnums(assemblies);
            SearchTables(assemblies);
            SearchUnions(assemblies);
        }

        private void SearchEnums(IEnumerable<Assembly> assemblies)
        {
            Enums = new List<EnumDef>();

            SearchAssemblies(assemblies, type =>
            {
                if (!type.IsEnum)
                    return;

                // TODO Validate enum values must be specified in ascending order with a start value 0

                var attr = type.GetCustomAttribute<BehaveAsEnumAttribute>(false);
                if (attr != null)
                    Enums.Add(new EnumDef(type));
            });
        }

        private void SearchTables(IEnumerable<Assembly> assemblies)
        {
            Tables = new List<TableDef>();

            SearchAssemblies(assemblies, type =>
            {
                var attr = type.GetCustomAttribute<BehaveAsTableAttribute>(false);
                if (attr != null)
                    Tables.Add(new TableDef(type));
            });
        }

        private void SearchUnions(IEnumerable<Assembly> assemblies)
        {
            var unions = new SortedList<string, UnionDef>();

            SearchAssemblies(assemblies, type =>
            {
                var attr = type.GetCustomAttribute<BehaveAsUnionIncludeAttribute>(false);
                if (attr != null)
                {
                    var unionType = attr.UnionType;
                    UnionDef unionDef;
                    if (!unions.TryGetValue(unionType.FullName, out unionDef))
                    {
                        unionDef = new UnionDef(unionType);
                        unions.Add(unionType.FullName, unionDef);
                    }

                    unionDef.IncludedTypes.Add(Tuple.Create(type, attr.Tag));
                }
            });

            foreach (var union in unions.Values)
                union.Sort();

            Unions = new List<UnionDef>(unions.Values);
        }

        private static void SearchAssemblies(IEnumerable<Assembly> assemblies, Action<Type> onType)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    onType(type);
                }
            }
        }
    }
}
