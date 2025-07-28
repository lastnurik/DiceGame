using DiceGame.Domain;
using DiceGame.Services;
using DiceGame.UI;
using DiceGame.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGame
{
    public static class Constants
    {
        public const int DICE_FACES_COUNT = 6;
        public const int FIRST_PLAYER_MIN_VALUE = 0;
        public const int FIRST_PLAYER_MAX_VALUE = 1;
        public const int DICE_ROLL_MIN_VALUE = 0;
        public const int DICE_ROLL_MAX_VALUE = 5;
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Dice> availableDice;
            UserInterface ui = new UserInterface();
            HmacService hmacService = new HmacService();
            FairPlayProtocol fairPlay = new FairPlayProtocol(hmacService, ui);
            ProbabilityCalculator probabilityCalculator = new ProbabilityCalculator();
            ProbabilityTableGenerator probabilityTableGenerator = new ProbabilityTableGenerator(ui, probabilityCalculator);

            try
            {
                availableDice = DiceParser.ParseArgs(args);
            }
            catch (ArgumentException ex)
            {
                ui.DisplayError(ex.Message);
                Environment.Exit(1);
                return;
            }

            ui.DisplayMessage("Successfully parsed dice:");
            for (int i = 0; i < availableDice.Count; i++)
            {
                ui.DisplayMessage($"{i}: {availableDice[i]}");
            }

            int firstMoveChoiceResult;
            do
            {
                firstMoveChoiceResult = fairPlay.DetermineFirstPlayerFairly(Constants.FIRST_PLAYER_MIN_VALUE, Constants.FIRST_PLAYER_MAX_VALUE);
                if (firstMoveChoiceResult == (int)UserInputStatus.Exit)
                {
                    Environment.Exit(0);
                }
                if (firstMoveChoiceResult == (int)UserInputStatus.Help)
                {
                    ui.DisplayMessage("This phase determines who starts. The computer has chosen a secret number (0 or 1) and shown its HMAC. Guess the number. If your guess matches, you start. Otherwise, the computer starts. The computer will reveal its number and key after your guess, so you can verify fairness.");
                }
            } while (firstMoveChoiceResult == (int)UserInputStatus.Help);

            bool computerGoesFirst = (firstMoveChoiceResult == 1);
            ui.DisplayMessage($"{(computerGoesFirst ? "I" : "You")} make the first move.");

            Dice computerDice = null;
            Dice userDice = null;
            List<Dice> remainingDice = new List<Dice>(availableDice);

            (computerDice, userDice) = SelectPlayersDice(ui, probabilityCalculator, probabilityTableGenerator, availableDice, remainingDice, computerGoesFirst);
            if (computerDice == null || userDice == null) Environment.Exit(0);

            ui.DisplayMessage("\nIt's time for my roll.");
            int computerRollFairIndex;
            do
            {
                computerRollFairIndex = fairPlay.PerformFairValueGeneration(Constants.DICE_ROLL_MIN_VALUE, Constants.DICE_ROLL_MAX_VALUE);
                if (computerRollFairIndex == (int)UserInputStatus.Exit) Environment.Exit(0);
                if (computerRollFairIndex == (int)UserInputStatus.Help)
                {
                    ui.DisplayMessage("This protocol ensures fair random number generation. The computer commits to a number by showing its HMAC. You then choose a number. The final result is a modular sum of both, and the computer reveals its number and key, allowing you to verify the HMAC.");
                    probabilityTableGenerator.DisplayTable(availableDice);
                }
            } while (computerRollFairIndex == (int)UserInputStatus.Help);

            int computerRollResult = computerDice.Roll(computerRollFairIndex);
            ui.DisplayMessage($"My roll result is {computerRollResult}.");

            ui.DisplayMessage("\nIt's time for your roll.");
            int userRollFairIndex;
            do
            {
                userRollFairIndex = fairPlay.PerformFairValueGeneration(Constants.DICE_ROLL_MIN_VALUE, Constants.DICE_ROLL_MAX_VALUE);
                if (userRollFairIndex == (int)UserInputStatus.Exit) Environment.Exit(0);
                if (userRollFairIndex == (int)UserInputStatus.Help)
                {
                    ui.DisplayMessage("This protocol ensures fair random number generation. The computer commits to a number by showing its HMAC. You then choose a number. The final result is a modular sum of both, and the computer reveals its number and key, allowing you to verify the HMAC.");
                    probabilityTableGenerator.DisplayTable(availableDice);
                }
            } while (userRollFairIndex == (int)UserInputStatus.Help);

            int userRollResult = userDice.Roll(userRollFairIndex);
            ui.DisplayMessage($"Your roll result is {userRollResult}.");

            if (computerRollResult > userRollResult)
            {
                ui.DisplayMessage($"I win ({computerRollResult} > {userRollResult})!");
            }
            else if (userRollResult > computerRollResult)
            {
                ui.DisplayMessage($"You win ({userRollResult} > {computerRollResult})!");
            }
            else
            {
                ui.DisplayMessage($"It's a draw ({computerRollResult} = {userRollResult})!");
            }
        }

        static (Dice computerDice, Dice userDice) SelectPlayersDice(
            UserInterface ui,
            ProbabilityCalculator calculator,
            ProbabilityTableGenerator ptg,
            List<Dice> allAvailableDice,
            List<Dice> remainingDice,
            bool computerGoesFirst)
        {
            Dice computerDice = null;
            Dice userDice = null;

            if (computerGoesFirst)
            {
                computerDice = SelectComputerDice(remainingDice, calculator);
                remainingDice.Remove(computerDice);
                ui.DisplayMessage($"I make the first move and choose the {computerDice} dice.");

                UserInput userChoice;
                do
                {
                    userChoice = SelectUserDice(ui, remainingDice);
                    if (userChoice.Status == UserInputStatus.Exit) return (null, null);
                    if (userChoice.Status == UserInputStatus.Help) ptg.DisplayTable(allAvailableDice);
                } while (userChoice.Status == UserInputStatus.Help);
                userDice = remainingDice[userChoice.Value];
                ui.DisplayMessage($"You choose the {userDice} dice.");
            }
            else
            {
                UserInput userChoice;
                do
                {
                    userChoice = SelectUserDice(ui, remainingDice);
                    if (userChoice.Status == UserInputStatus.Exit) return (null, null);
                    if (userChoice.Status == UserInputStatus.Help) ptg.DisplayTable(allAvailableDice);
                } while (userChoice.Status == UserInputStatus.Help);
                userDice = remainingDice[userChoice.Value];
                remainingDice.Remove(userDice);
                ui.DisplayMessage($"You make the first move and choose the {userDice} dice.");

                computerDice = SelectComputerDice(remainingDice, calculator);
                ui.DisplayMessage($"I choose the {computerDice} dice.");
            }
            return (computerDice, userDice);
        }

        static UserInput SelectUserDice(UserInterface ui, List<Dice> currentAvailableDice)
        {
            List<string> diceOptions = currentAvailableDice.Select(d => d.ToString()).ToList();
            return ui.GetMenuSelection("Choose your dice", diceOptions);
        }

        static Dice SelectComputerDice(List<Dice> availableDice, ProbabilityCalculator calculator)
        {
            if (availableDice.Count == 1)
            {
                return availableDice[0];
            }

            Dice bestComputerDice = null;
            double maxMinWinProbability = -1.0;

            foreach (var compCandidate in availableDice)
            {
                double minWinProbForThisCandidate = 1.0;

                foreach (var userCandidate in availableDice.Where(d => d != compCandidate))
                {
                    double probCompBeatsUser = calculator.CalculateWinProbability(compCandidate, userCandidate);
                    if (probCompBeatsUser < minWinProbForThisCandidate)
                    {
                        minWinProbForThisCandidate = probCompBeatsUser;
                    }
                }

                if (minWinProbForThisCandidate > maxMinWinProbability)
                {
                    maxMinWinProbability = minWinProbForThisCandidate;
                    bestComputerDice = compCandidate;
                }
            }
            return bestComputerDice;
        }
    }
}