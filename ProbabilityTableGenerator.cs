using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiceGame
{
    public class ProbabilityTableGenerator
    {
        private readonly UserInterface _ui;
        private readonly ProbabilityCalculator _calculator;

        public ProbabilityTableGenerator(UserInterface ui, ProbabilityCalculator calculator)
        {
            _ui = ui;
            _calculator = calculator;
        }

        public void DisplayTable(List<Dice> diceList)
        {
            if (diceList == null || diceList.Count == 0)
            {
                _ui.DisplayMessage("No dice available to generate probability table.");
                return;
            }

            int maxDiceNameWidth = diceList.Max(d => d.ToString().Length);
            int columnWidth = Math.Max(maxDiceNameWidth, 8);

            StringBuilder sb = new StringBuilder();

            sb.Append("".PadRight(columnWidth)).Append("|");
            for (int i = 0; i < diceList.Count; i++)
            {
                sb.Append($" Die {i}".PadLeft(columnWidth)).Append("|");
            }
            sb.Remove(sb.Length - 1, 1).AppendLine();

            sb.Append("".PadRight(columnWidth, '-')).Append("+");
            for (int i = 0; i < diceList.Count; i++)
            {
                sb.Append("".PadRight(columnWidth, '-')).Append("+");
            }
            sb.Remove(sb.Length - 1, 1).AppendLine();

            for (int i = 0; i < diceList.Count; i++)
            {
                sb.Append($"Die {i}".PadRight(columnWidth)).Append("|");
                for (int j = 0; j < diceList.Count; j++)
                {
                    if (i == j)
                    {
                        sb.Append("  N/A".PadLeft(columnWidth)).Append("|");
                    }
                    else
                    {
                        double prob = _calculator.CalculateWinProbability(diceList[i], diceList[j]);
                        sb.Append($" {prob:P1}".PadLeft(columnWidth)).Append("|");
                    }
                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }

            _ui.DisplayMessage(sb.ToString());
        }
    }
}