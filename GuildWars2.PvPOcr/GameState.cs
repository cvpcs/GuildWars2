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

            public bool TryProcessScore(string newScoreText)
            {
                if (int.TryParse(newScoreText.Trim()
                                          .ToLower()
                                          .Replace('o', '0'),
                                 out int newScore))
                {
                    int scoreDelta = newScore - Score;

                    if (Score < 10 && scoreDelta > 50)
                    {
                        // ignore faulty read where +5 for kill probably triggered a delta > 50 (e.g. +5 7 -> 57)
                        return false;
                    }

                    if (scoreDelta > 178)
                    {
                        // ignore faulty read where +5 for kill triggered a score that is greater than the highest
                        // maximum tick value (e.g. 78 +5 -> 785). maximum tick is:
                        //   150 for lord kill
                        //    25 for 5 player kills
                        //     3 for 3 node ticks
                        return false;
                    }

                    Score = newScore;

                    if (scoreDelta > 0 && scoreDelta < 15)
                    {
                        Kills += scoreDelta / 5;
                    }

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                Score = 0;
                Kills = 0;
            }
        }
    }
}
