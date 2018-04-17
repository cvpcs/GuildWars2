namespace GuildWars2.PvPCasterToolbox
{
    public interface IGameState
    {
        ITeamState Red { get; }
        ITeamState Blue { get; }
    }
}
