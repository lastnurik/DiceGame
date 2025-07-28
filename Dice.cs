using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGame
{
    public class Dice
    {
        public List<int> Faces { get; }

        public Dice(IEnumerable<int> faces)
        {
            var faceList = faces.ToList();
            if (faceList.Count != Constants.DICE_FACES_COUNT)
            {
                throw new ArgumentException($"A dice must have exactly {Constants.DICE_FACES_COUNT} faces.");
            }

            Faces = faceList;
        }

        public int Roll(int index)
        {
            if (index < 0 || index >= Constants.DICE_FACES_COUNT)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {Constants.DICE_FACES_COUNT - 1}.");
            }

            return Faces[index];
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Faces)}]";
        }

        public override bool Equals(object obj)
        {
            if (obj is Dice other)
            {
                return Faces.SequenceEqual(other.Faces);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Faces.Aggregate(0, (hash, face) => hash ^ face.GetHashCode());
        }
    }
}