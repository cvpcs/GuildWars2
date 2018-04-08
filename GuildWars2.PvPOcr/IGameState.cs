namespace GuildWars2.PvPOcr
{
    public interface IGameState
    {
        ITeamState Red { get; }
        ITeamState Blue { get; }
    }
}
