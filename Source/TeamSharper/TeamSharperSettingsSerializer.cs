using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace C24.TeamSharper
{
    public class TeamSharperSettingsSerializer : ITeamSharperSettingsSerializer
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public TeamSharperSettings Load(string settingsFilePath)
        {
            string json = File.ReadAllText(settingsFilePath);
            TeamSharperSettings teamSharperSettings = JsonConvert.DeserializeObject<TeamSharperSettings>(json, settings);
            teamSharperSettings.FilePath = settingsFilePath; // Inject the own file path.
            return teamSharperSettings;
        }
    }
}
