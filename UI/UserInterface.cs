using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGame.UI
{
    public enum UserInputStatus
    {
        Success = 0,
        Exit = -1,
        Help = -2,
        Invalid = -3
    }

    public struct UserInput
    {
        public UserInputStatus Status { get; }
        public int Value { get; }

        public UserInput(UserInputStatus status, int value = 0)
        {
            Status = status;
            Value = value;
        }
    }

    public class UserInterface
    {
        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("ERROR: " + message);
            Console.ResetColor();
        }

        public string GetUserInput(string prompt)
        {
            Console.Write(prompt + ": ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        public UserInput GetMenuSelection(string prompt, List<string> options, string exitOption = "X", string helpOption = "?")
        {
            while (true)
            {
                DisplayMessage($"\n{prompt}:");
                for (int i = 0; i < options.Count; i++)
                {
                    DisplayMessage($"{i} - {options[i]}");
                }
                DisplayMessage($"{exitOption} - exit");
                DisplayMessage($"{helpOption} - help");

                string input = GetUserInput("Your selection").ToUpper();

                if (input == exitOption)
                {
                    return new UserInput(UserInputStatus.Exit);
                }
                if (input == helpOption)
                {
                    return new UserInput(UserInputStatus.Help);
                }

                if (int.TryParse(input, out int selection) && selection >= 0 && selection < options.Count)
                {
                    return new UserInput(UserInputStatus.Success, selection);
                }
                else
                {
                    DisplayError("Invalid selection. Please choose a number from the list, 'X' to exit, or '?' for help.");
                }
            }
        }

        public UserInput GetNumericalInput(string prompt, int minValue, int maxValue, string exitOption = "X", string helpOption = "?")
        {
            while (true)
            {
                DisplayMessage($"\n{prompt} (range {minValue}..{maxValue}):");
                for (int i = minValue; i <= maxValue; i++)
                {
                    DisplayMessage($"{i} - {i}");
                }
                DisplayMessage($"{exitOption} - exit");
                DisplayMessage($"{helpOption} - help");

                string input = GetUserInput("Your selection").ToUpper();

                if (input == exitOption)
                {
                    return new UserInput(UserInputStatus.Exit);
                }
                if (input == helpOption)
                {
                    return new UserInput(UserInputStatus.Help);
                }

                if (int.TryParse(input, out int selection) && selection >= minValue && selection <= maxValue)
                {
                    return new UserInput(UserInputStatus.Success, selection);
                }
                else
                {
                    DisplayError($"Invalid selection. Please choose a number between {minValue} and {maxValue}, 'X' to exit, or '?' for help.");
                }
            }
        }
    }
}