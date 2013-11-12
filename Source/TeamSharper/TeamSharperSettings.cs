using System.Collections.ObjectModel;

using Newtonsoft.Json;

namespace C24.TeamSharper
{
    public sealed class TeamSharperSettings
    {
        /// <summary>
        /// Gets or sets the file path for the current TeamSharper settings instance.
        /// </summary>
        /// <remarks>
        /// This property is not part of the settings file content, of course. It gets injected during deserialization.
        /// </remarks>
        [JsonIgnore]
        public string FilePath { get; set; }

        public Collection<TeamSharperSettingsLayer> Layers { get; set; }
    }
}
