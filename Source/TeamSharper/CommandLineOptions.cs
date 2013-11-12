using System;
using System.IO;
using System.Reflection;

using CommandLine;
using CommandLine.Text;

namespace C24.TeamSharper
{
    internal sealed class CommandLineOptions
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global

        [Option('s', "settings", HelpText = "Absolute or relative path to the TeamSharper settings file.", Required = true)]
        public string SettingsPath { get; set; }

        [Option('d', "directory", HelpText = "Absolute or relative path to the root directory for searching solution files.", Required = true)]
        public string SearchDirectory { get; set; }

        [Option('t', "test", HelpText = "Set this flag to enable test only mode (does not create or alter any files).")]
        public bool Test { get; set; }

        // ReSharper disable UnusedMember.Global

        [HelpOption('?', "help", HelpText = "Set this flag to show the help text.")]
        public string GetUsage()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            AssemblyName assemblyName = assembly.GetName();
            HelpText help = new HelpText
            {
                Heading = new HeadingInfo(assemblyName.Name, assemblyName.Version.ToString()),
                Copyright = new CopyrightInfo("CHECK24", 2013),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine(string.Format("Usage: {0} (-o|--option) value?", Path.GetFileName(assembly.Location)));
            help.AddOptions(this);
            help.AddPostOptionsLine(Environment.NewLine);

            return help;
        }

        // ReSharper restore UnusedMember.Global

        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }
}
