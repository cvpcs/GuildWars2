namespace GuildWars2.PvPOcr
{
    internal class GameState : IGameState
    {
        private TeamState red = new TeamState();
        public ITeamState Red => red;

        private TeamState blue = new TeamState();
        public ITeamState Blue => blue;

        public bool TryProcessScores(string newRedScoreText, string newBlueScoreText)
        {
            bool result = this.red.TryProcessScore(newRedScoreText) && this.blue.TryProcessScore(newBlueScoreText);

            if (result && this.red.Score == 0 && this.blue.Score == 0)
            {
                this.red.Reset();
                this.blue.Reset();
            }

            return result;
        }

        private class TeamState : ITeamState
        {
            public int Score { get; private set; } = -1;
            public double ScorePercentage => Score / 500.0;

            public int Kills { get; private set; } = 0;

            public int ScoreDelta { get; private set; } = 0;
            public int KillsDelta { get; private set; } = 0;

            private int consecutiveInvalidDeltaFailures = 0;

            public bool TryProcessScore(string newScoreText)
            {
                if (int.TryParse(newScoreText.Trim()
                                          .ToLower()
                                          .Replace('o', '0'),
                                 out int newScore))
                {
                    // if the score is < 0, we haven't read a score yet so assume delta of 0 and just accept the new score
                    int scoreDelta = Score >= 0 ? newScore - Score : 0;

                    // ignore faulty read where +5 for kill could be read as part of the score:
                    //   - delta > 50 when last score < 10 (e.g. +5 9 -> 59)
                    //   - delta > 178 since maximum tick score should be 178 (+150 lord, +25 5x kill, +3 3x node)
                    // this is overridden and the score is considered valid if we fail here 10 times in a row in order
                    // to catch cases where we have swapped games and the deltas are unreliable
                    if (((Score < 10 && scoreDelta > 50) ||
                         scoreDelta > 178) &&
                        this.consecutiveInvalidDeltaFailures++ < 10)
                    {
                        return false;
                    }

                    this.consecutiveInvalidDeltaFailures = 0;

                    ScoreDelta = scoreDelta;
                    Score = newScore;

                    // if our score delta is between 0 and 15, calculate possible kill-related increases
                    KillsDelta = (scoreDelta > 0 && scoreDelta < 15) ? scoreDelta / 5 : 0;
                    Kills += KillsDelta;

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                ScoreDelta = 0;
                Score = 0;

                KillsDelta = 0;
                Kills = 0;
            }
        }
    }
}
