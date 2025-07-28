using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace DiceGame
{
    public class ProbabilityCalculator
    {
        private readonly ConcurrentDictionary<(string, string), double> _winProbabilityCache = new ConcurrentDictionary<(string, string), double>();

        public double CalculateWinProbability(Dice playerDice, Dice opponentDice)
        {
            string playerKey = playerDice.ToString();
            string opponentKey = opponentDice.ToString();

            if (_winProbabilityCache.TryGetValue((playerKey, opponentKey), out double cachedProb))
            {
                return cachedProb;
            }

            int playerWins = 0;
            int totalOutcomes = playerDice.Faces.Count * opponentDice.Faces.Count;

            foreach (int playerFace in playerDice.Faces)
            {
                foreach (int opponentFace in opponentDice.Faces)
                {
                    if (playerFace > opponentFace)
                    {
                        playerWins++;
                    }
                }
            }

            double prob = (double)playerWins / totalOutcomes;
            _winProbabilityCache.TryAdd((playerKey, opponentKey), prob);

            return prob;
        }
    }
}