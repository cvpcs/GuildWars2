namespace GuildWars2.PvPCasterToolbox.GameState
{
    public interface IGameState
    {
        ITeamState Red { get; }
        ITeamState Blue { get; }
    }
}
