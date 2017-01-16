using BehaveAsSakura.SerializationCompiler.Schema;
using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BehaveAsSakura.SerializationCompiler
{
    class Program
    {
        public static Options Options { get; private set; }

        private static void Main(string[] args)
        {
            Options = new Options();

            var isValid = Parser.Default.ParseArguments(args, Options);
            if (isValid)
            {
                var assemblies = LoadAssemblies();
                var schema = new SchemaDef("BehaveAsSakura.Serialization", assemblies);
                var code = SchemaWriter.ToString(schema);

                Console.WriteLine(code);
            }

            Console.ReadKey();
        }

        private static IEnumerable<Assembly> LoadAssemblies()
        {
            yield return typeof(BehaviorTreeDesc).Assembly;

            foreach (var p in Options.InputAssemblies)
            {
                var path = Path.Combine(Environment.CurrentDirectory, p);
                var assembly = Assembly.LoadFrom(path);

                yield return assembly;
            }
        }
    }
}
