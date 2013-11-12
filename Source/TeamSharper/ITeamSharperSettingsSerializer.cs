namespace C24.TeamSharper
{
    public interface ITeamSharperSettingsSerializer
    {
        TeamSharperSettings Load(string settingsFilePath);
    }
}
