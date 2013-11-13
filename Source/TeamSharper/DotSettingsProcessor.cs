using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace C24.TeamSharper
{
    public sealed class DotSettingsProcessor
    {
        private const string absolutePathDummy = @"B:\AbsoluteFloppyDiscPath\VeryUnlikely\Dummy.DotSettings";

        private readonly string teamSharperSettingsFile;
        private readonly string solutionSearchRootDirectory;
        private readonly IDotSettingsSerializer dotSettingsSerializer;
        private readonly ITeamSharperSettingsSerializer teamSharperSettingsSerializer;

        public DotSettingsProcessor(string teamSharperSettingsFile, string solutionSearchRootDirectory)
            : this(teamSharperSettingsFile, solutionSearchRootDirectory, new DotSettingsSerializer(), new TeamSharperSettingsSerializer())
        {
        }

        public DotSettingsProcessor(
            string teamSharperSettingsFile,
            string solutionSearchRootDirectory,
            IDotSettingsSerializer dotSettingsSerializer,
            ITeamSharperSettingsSerializer teamSharperSettingsSerializer)
        {
            if (teamSharperSettingsFile == null)
            {
                throw new ArgumentNullException("teamSharperSettingsFile");
            }

            if (solutionSearchRootDirectory == null)
            {
                throw new ArgumentNullException("solutionSearchRootDirectory");
            }

            if (dotSettingsSerializer == null)
            {
                throw new ArgumentNullException("dotSettingsSerializer");
            }

            if (teamSharperSettingsSerializer == null)
            {
                throw new ArgumentNullException("teamSharperSettingsSerializer");
            }

            this.teamSharperSettingsFile = teamSharperSettingsFile;
            this.solutionSearchRootDirectory = solutionSearchRootDirectory;
            this.dotSettingsSerializer = dotSettingsSerializer;
            this.teamSharperSettingsSerializer = teamSharperSettingsSerializer;
        }

        public IEnumerable<Change> CalculateChanges()
        {
            TeamSharperSettings teamSharperSettings = this.teamSharperSettingsSerializer.Load(this.teamSharperSettingsFile);
            List<DotSettings> dotSettings = this.dotSettingsSerializer.LoadAll(solutionSearchRootDirectory).ToList();

            return dotSettings
                .Where(settings => !settings.FileExists || !AreEqual(teamSharperSettings, settings))
                .Select(settings => new Change(
                    string.Format("{0} file '{1}'", settings.FileExists ? "Modifiy" : "Create", settings.FilePath),
                    () => this.UpdateDotSettings(settings, teamSharperSettings)));
        }

        private bool AreEqual(TeamSharperSettings teamSharperSettings, DotSettings dotSettings)
        {
            // We require the absolute path to be our exact dummy to enshure constistency:
            if (dotSettings.Layers.Any(x => !x.AbsolutePath.Equals(absolutePathDummy, StringComparison.Ordinal)))
            {
                return false;
            }

            var absoluteTeamSettingsLayersPaths = teamSharperSettings.Layers
                .OrderBy(x => x.RelativePriority)
                .Select(x => x.RelativePath)
                .Select(x => PathHelper.MakeFilePathAbsoluteToDirectory(x, teamSharperSettings.FilePath))
                .Select(x => x.ToLowerInvariant());

            var absoluteDotSettingsLayersPaths = dotSettings.Layers
                .OrderBy(x => x.RelativePriority)
                .Select(x => x.RelativePath)
                .Select(x => PathHelper.MakeFilePathAbsoluteToDirectory(x, dotSettings.FilePath))
                .Select(x => x.ToLowerInvariant());

            return absoluteTeamSettingsLayersPaths.SequenceEqual(absoluteDotSettingsLayersPaths);
        }

        private void UpdateDotSettings(DotSettings dotSettings, TeamSharperSettings teamSharperSettings)
        {
            AdjustPathsInDotSettings(dotSettings, teamSharperSettings);

            // Finally, save the settings:
            this.dotSettingsSerializer.Save(dotSettings);
        }

        internal void AdjustPathsInDotSettings(DotSettings dotSettings, TeamSharperSettings teamSharperSettings)
        {
            dotSettings.Layers.Clear();
            foreach (TeamSharperSettingsLayer teamSharperSettingsLayer in teamSharperSettings.Layers.OrderBy(x => x.RelativePriority))
            {
                string layerReferenceAbsolute = PathHelper.MakeFilePathAbsoluteToDirectory(teamSharperSettingsLayer.RelativePath, teamSharperSettings.FilePath);
                string layerReferenceRelativeToSolution = PathHelper.MakeFilePathRelativeToDirectory(layerReferenceAbsolute, dotSettings.FilePath);

                dotSettings.Layers.Add(new DotSettingsLayer
                {
                    Id = teamSharperSettingsLayer.Id,
                    AbsolutePath = absolutePathDummy,
                    RelativePriority = teamSharperSettingsLayer.RelativePriority,
                    RelativePath = layerReferenceRelativeToSolution
                });
            }
        }
    }
}
