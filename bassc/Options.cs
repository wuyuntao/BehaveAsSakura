using CommandLine;
using CommandLine.Text;

namespace BehaveAsSakura.SerializationCompiler
{
    enum SerializerType
    {
        FlatBuffers,
        Json,
    }

    class Options
    {
        [OptionArray('i', "input-assemblies", Required = true)]
        public string[] InputAssemblies { get; set; }

        [Option('V', "verbose", DefaultValue = true)]
        public bool Verbose { get; set; }

        [Option('t', "serializer-type", DefaultValue = SerializerType.FlatBuffers)]
        public SerializerType SerializerType { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
