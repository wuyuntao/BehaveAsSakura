using CommandLine;
using CommandLine.Text;

namespace BehaveAsSakura.SerializationCompiler
{
    class Options
    {
        [OptionArray('i', "input-assemblies", Required = true)]
        public string[] InputAssemblies { get; set; }

        [OptionArray('c', "flatc-path", DefaultValue = "flatc.exe")]
        public string FlatcPath { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
