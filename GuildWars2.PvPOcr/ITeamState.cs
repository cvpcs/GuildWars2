namespace GuildWars2.PvPOcr
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
