using System;
using System.Text;

namespace DiceGame
{
    public class FairPlayProtocol
    {
        private readonly HmacService _hmacService;
        private readonly UserInterface _ui;

        public FairPlayProtocol(HmacService hmacService, UserInterface ui)
        {
            _hmacService = hmacService;
            _ui = ui;
        }

        public int DetermineFirstPlayerFairly(int minValue, int maxValue)
        {
            _ui.DisplayMessage("\nLet's determine who makes the first move.");

            byte[] secretKey = _hmacService.GenerateSecureKey();
            int computerNumber = _hmacService.GenerateSecureRandomInt(minValue, maxValue);

            byte[] computerNumberBytes = _hmacService.IntToBytes(computerNumber);
            string hmacValue = _hmacService.ComputeHmacSha3(secretKey, computerNumberBytes);

            _ui.DisplayMessage($"I selected a random value in the range {minValue}..{maxValue} (HMAC={hmacValue}).");
            _ui.DisplayMessage("Try to guess my selection.");

            UserInput userInput = _ui.GetNumericalInput("Your selection", minValue, maxValue);

            if (userInput.Status == UserInputStatus.Exit) return (int)UserInputStatus.Exit;
            if (userInput.Status == UserInputStatus.Help) return (int)UserInputStatus.Help;

            _ui.DisplayMessage($"My selection: {computerNumber} (KEY={_hmacService.ByteArrayToHex(secretKey)}).");

            if (userInput.Value == computerNumber)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public int PerformFairValueGeneration(int minValue, int maxValue)
        {
            _ui.DisplayMessage($"\nInitiating Fair Random Number Generation (Range: {minValue} to {maxValue})");

            byte[] secretKey = _hmacService.GenerateSecureKey();
            int computerNumber = _hmacService.GenerateSecureRandomInt(minValue, maxValue);

            byte[] computerNumberBytes = _hmacService.IntToBytes(computerNumber);
            string hmacValue = _hmacService.ComputeHmacSha3(secretKey, computerNumberBytes);

            _ui.DisplayMessage($"I selected a random value in the range {minValue}..{maxValue} (HMAC={hmacValue}).");

            UserInput userInput = _ui.GetNumericalInput($"Add your number modulo {maxValue - minValue + 1}", minValue, maxValue);

            if (userInput.Status == UserInputStatus.Exit) return (int)UserInputStatus.Exit;
            if (userInput.Status == UserInputStatus.Help) return (int)UserInputStatus.Help;

            _ui.DisplayMessage($"My number is {computerNumber} (KEY={_hmacService.ByteArrayToHex(secretKey)}).");

            int rangeSize = maxValue - minValue + 1;
            int resultValue = (computerNumber + userInput.Value) % rangeSize;

            _ui.DisplayMessage($"The fair number generation result is {computerNumber} + {userInput.Value} = {resultValue} (mod {rangeSize}).");
            _ui.DisplayMessage("Fair Random Number Generation Complete");

            return resultValue;
        }
    }
}