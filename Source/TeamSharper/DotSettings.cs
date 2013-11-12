using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace C24.TeamSharper
{
    public sealed class DotSettings
    {
        private readonly string filePath;
        private readonly bool fileExists;
        private readonly Collection<DotSettingsLayer> layers;

        public DotSettings(string filePath, IEnumerable<DotSettingsLayer> layers, bool fileExists)
        {
            this.filePath = filePath;
            this.layers = new Collection<DotSettingsLayer>(layers.ToList());
            this.fileExists = fileExists;
        }

        public string FilePath
        {
            get { return this.filePath; }
        }

        public bool FileExists
        {
            get { return this.fileExists; }
        }

        public Collection<DotSettingsLayer> Layers
        {
            get { return this.layers; }
        }
    }
}
