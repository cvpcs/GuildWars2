namespace GuildWars2.PvPCasterToolbox
{
    public interface ITeamState
    {
        int Score { get; }
        double ScorePercentage { get; }

        int Kills { get; }

        int ScoreDelta { get; }
        int KillsDelta { get; }
    }
}
