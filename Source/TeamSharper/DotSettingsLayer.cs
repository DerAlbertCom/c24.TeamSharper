using System;

namespace C24.TeamSharper
{
    public sealed class DotSettingsLayer
    {
        public Guid Id { get; set; }
        public string AbsolutePath { get; set; }
        public string RelativePath { get; set; }
        public double RelativePriority { get; set; }
    }
}
