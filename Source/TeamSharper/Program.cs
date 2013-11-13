using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using CommandLine;

namespace C24.TeamSharper
{
    internal static class Program
    {
        internal static int Main(string[] args)
        {
            CommandLineOptions commandLineOptions = new CommandLineOptions();
            if (Parser.Default.ParseArguments(args, commandLineOptions))
            {
                try
                {
                    Execute(commandLineOptions);
                    return 0;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            return -1;
        }

        private static void Execute(CommandLineOptions commandLineOptions)
        {
            string applicationDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            // Expand the paths to fully qualified paths:
            string settingsFile = PathHelper.MakeFilePathAbsoluteToDirectory(commandLineOptions.SettingsPath, applicationDirectory);
            string rootDirectory = PathHelper.MakeFilePathAbsoluteToDirectory(commandLineOptions.SearchDirectory, applicationDirectory);

            List<Change> changes = new DotSettingsProcessor(settingsFile, rootDirectory).CalculateChanges().ToList();

            if (changes.Any())
            {
                Console.WriteLine("The following changes {0} applied:", commandLineOptions.Test ? "would be" : "are");
                foreach (Change change in changes)
                {
                    Console.WriteLine("-> {0}", change.Description);
                    if (!commandLineOptions.Test)
                    {
                        change.Apply();
                    }
                }
            }
            else
            {
                Console.WriteLine("No changes {0} applied, you're already up to date!", commandLineOptions.Test ? "would be" : "are");
            }
        }
    }
}
