using System.Collections.Generic;

namespace C24.TeamSharper
{
    public interface IDotSettingsSerializer
    {
        void Save(DotSettings dotSettings);
        IEnumerable<DotSettings> LoadAll(string rootDirectory);
    }
}
