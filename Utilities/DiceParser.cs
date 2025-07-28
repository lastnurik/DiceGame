using DiceGame.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGame.Utilities
{
    public static class DiceParser
    {
        public static List<Dice> ParseArgs(string[] args)
        {
            if (args.Length < 3)
            {
                throw new ArgumentException("You must specify at least 3 dice. Example: dotnet run 2,2,4,4,9,9 6,8,1,1,8,6 7,5,3,7,5,3");
            }

            var diceList = new List<Dice>();

            foreach (var arg in args)
            {
                var faces = arg.Split(',').Select(int.Parse).ToList();
                if (faces.Count != Constants.DICE_FACES_COUNT)
                {
                    throw new ArgumentException($"Each dice must have exactly {Constants.DICE_FACES_COUNT} faces.");
                }

                diceList.Add(new Dice(faces));
            }

            return diceList;
        }
    }
}