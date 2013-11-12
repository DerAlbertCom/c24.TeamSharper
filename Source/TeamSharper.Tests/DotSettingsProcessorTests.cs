using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Moq;

using NUnit.Framework;

namespace C24.TeamSharper.Tests
{
    [TestFixture]
    public class DotSettingsProcessorTests
    {
        [Test]
        public void Test_if_CalculateChanges_produces_the_correct_description()
        {
            // Arrange:
            const string solutionSearchRootDirectory = @"C:\SomeDir\Solutions\";
            const string teamSharperSettingsFile = @"C:\SomeDir\TeamSettings\TeamSettings.json";
            DotSettings dotSettings = new DotSettings(@"C:\Solutions\SomeSolution.sln.DotSettings", Enumerable.Empty<DotSettingsLayer>(), false);
            TeamSharperSettings teamSharperSettings = new TeamSharperSettings
            {
                FilePath = teamSharperSettingsFile,
                Layers = new Collection<TeamSharperSettingsLayer>
                {
                    new TeamSharperSettingsLayer
                    {
                        Id = Guid.NewGuid(),
                        RelativePath = @"..\Test.DotSettings",
                        RelativePriority = 1
                    }
                }
            };

            MockRepository repo = new MockRepository(MockBehavior.Strict);
            Mock<IDotSettingsSerializer> dotSettingsSerializer = repo.Create<IDotSettingsSerializer>();
            Mock<ITeamSharperSettingsSerializer> teamSharperSettingsSerializer = repo.Create<ITeamSharperSettingsSerializer>();

            dotSettingsSerializer.Setup(x => x.LoadAll(solutionSearchRootDirectory)).Returns(new[] { dotSettings });
            teamSharperSettingsSerializer.Setup(x => x.Load(teamSharperSettingsFile)).Returns(teamSharperSettings);

            DotSettingsProcessor processor = new DotSettingsProcessor(
                teamSharperSettingsFile,
                solutionSearchRootDirectory,
                dotSettingsSerializer.Object,
                teamSharperSettingsSerializer.Object);

            // Act:
            List<Change> changes = processor.CalculateChanges().ToList();

            // Assert:
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(@"Create file 'C:\Solutions\SomeSolution.sln.DotSettings'", changes.First().Description);
        }

        [Test]
        public void Test_if_AdjustPathsInDotSettings_produces_correct_paths()
        {
            // Arrange:
            const string solutionSearchRootDirectory = @"C:\dev\src\solutions";
            const string teamSharperSettingsFile = @"C:\dev\config\team\TeamSettings.json";
            DotSettings dotSettings = new DotSettings(@"C:\dev\src\solutions\SomeSolution.sln.DotSettings", Enumerable.Empty<DotSettingsLayer>(), false);
            TeamSharperSettings teamSharperSettings = new TeamSharperSettings
            {
                FilePath = teamSharperSettingsFile,
                Layers = new Collection<TeamSharperSettingsLayer>
                {
                    new TeamSharperSettingsLayer
                    {
                        Id = Guid.NewGuid(),
                        RelativePath = @"layers\ResharperSettings.Layer1.DotSettings",
                        RelativePriority = 1
                    },
                    new TeamSharperSettingsLayer
                    {
                        Id = Guid.NewGuid(),
                        RelativePath = @"layers\ResharperSettings.Layer2.DotSettings",
                        RelativePriority = 2
                    }
                }
            };

            MockRepository repo = new MockRepository(MockBehavior.Strict);
            Mock<IDotSettingsSerializer> dotSettingsSerializer = repo.Create<IDotSettingsSerializer>();
            Mock<ITeamSharperSettingsSerializer> teamSharperSettingsSerializer = repo.Create<ITeamSharperSettingsSerializer>();

            dotSettingsSerializer.Setup(x => x.LoadAll(solutionSearchRootDirectory)).Returns(new[] { dotSettings });
            teamSharperSettingsSerializer.Setup(x => x.Load(teamSharperSettingsFile)).Returns(teamSharperSettings);

            DotSettingsProcessor processor = new DotSettingsProcessor(
                teamSharperSettingsFile,
                solutionSearchRootDirectory,
                dotSettingsSerializer.Object,
                teamSharperSettingsSerializer.Object);

            // Act:
            processor.AdjustPathsInDotSettings(dotSettings, teamSharperSettings);

            // Assert:
            Assert.AreEqual(2, dotSettings.Layers.Count);
            Assert.AreEqual(@"..\..\config\team\layers\ResharperSettings.Layer1.DotSettings", dotSettings.Layers[0].RelativePath);
            Assert.AreEqual(@"..\..\config\team\layers\ResharperSettings.Layer2.DotSettings", dotSettings.Layers[1].RelativePath);
        }
    }
}
