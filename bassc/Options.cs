using CommandLine;
using CommandLine.Text;

namespace BehaveAsSakura.SerializationCompiler
{
    class Options
    {
        [OptionArray('i', "input-assemblies")]
        public string[] InputAssemblies { get; set; }

        [Option('c', "flatc-path", DefaultValue = "flatc.exe")]
        public string FlatcPath { get; set; }

        [Option('o', "output-path", DefaultValue = "BahveAsSakuraSerializer.cs")]
        public string OutputPath { get; set; }

        [Option('n', "namespace", DefaultValue = "BehaveAsSakura.Serialization")]
        public string Namespace { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
