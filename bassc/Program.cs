using BehaveAsSakura.SerializationCompiler.Schema;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var workPath = Environment.CurrentDirectory;

                var filename = $"Schema-{Guid.NewGuid()}";
                var schema = new SchemaDef("BehaveAsSakura.Serialization", assemblies);
                var schemaPath = Path.Combine(Environment.CurrentDirectory, $"{filename}.fbs");
                FlatBuffersSchemaWriter.ToFile(schema, schemaPath);

                var flatcPath = Path.Combine(Environment.CurrentDirectory, Options.FlatcPath);
                RunCommand(flatcPath, $@"-n --gen-onefile -o ""{workPath}"" ""{schemaPath}""");

                var csharpPath = Path.Combine(Environment.CurrentDirectory, $"{filename}.cs");

                var outputPath = Path.Combine(Environment.CurrentDirectory, Options.OutputPath);
                CSharpSerializerWriter.ToFile(schema, csharpPath, outputPath);
            }

#if DEBUG
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
#endif
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

        private static void RunCommand(string cmd, string args)
        {
            //Console.WriteLine("{0} {1}", cmd, args);

            var process = new Process();
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = cmd;
            processStartInfo.Arguments = args;
            process.StartInfo = processStartInfo;

            process.Start();
            process.WaitForExit();
        }
    }
}
