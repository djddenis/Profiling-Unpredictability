using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProject
{
    public static class StaticStrategies
    {
        static Random r = new Random();

        public static int Optimal(StrategyNode[] children, Prediction predict, Player player)
        {
            int ideal = predict.Value + 1;
            int nearest = -1;
            for (int i = Game.NUM_CARDS - 1; i >= ideal; i--)
                if (player.Cards[i] == Card.Held)
                    nearest = i;

            if (nearest >= ideal)
                return nearest;

            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (player.Cards[i] == Card.Held)
                    return i;
            throw new InvalidOperationException("No cards available to play");
        }

        public static int PlayerChoice(StrategyNode[] children, Prediction predict, Player player)
        {
            Console.WriteLine("Profiler's prediction: (" + predict.Value + ", " + predict.Certainty + ")");
            Console.Write("You have ");
            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (player.Cards[i] == Card.Held)
                    Console.Write(i + ", ");
            return int.Parse(Console.ReadLine());
        }

        public static int RandomWidening(StrategyNode[] children, Prediction predict, Player player)
        {
            if (player.CardsRemaining == 10)
                return r.Next(10);
            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (player.Cards[i] == Card.Played)
                    return r.Next(2) == 0 ? NearestTo(i, player) : NearestTo(i - 1, player);
            throw new InvalidOperationException("No cards available to play");
        }

        public static int LowestFirst(StrategyNode[] children, Prediction predict, Player player)
        {
            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (player.Cards[i] == Card.Held)
                    return i;
            throw new InvalidOperationException("No cards available to play");
        }

        public static int HighestFirst(StrategyNode[] children, Prediction predict, Player player)
        {
            for (int i = Game.NUM_CARDS - 1; i >= 0; i--)
                if (player.Cards[i] == Card.Held)
                    return i;
            throw new InvalidOperationException("No cards available to play");
        }

        public static int SkipFromLow(StrategyNode[] children, Prediction predict, Player player)
        {
            if (player.CardsRemaining == 10)
                return 0;
            int highestPlayed = 0;
            bool filledIn = false;
            for (int i = Game.NUM_CARDS - 1; i >= 0; i--)
            {
                if (player.Cards[i] == Card.Played)
                {
                    highestPlayed = i;
                    if (i == 0 || player.Cards[i - 1] == Card.Played)
                        filledIn = true;
                    break;
                }
            }
            return filledIn ? (highestPlayed == 8 ? 9 : highestPlayed + 2) : highestPlayed - 1;
        }

        public static int SkipFromHigh(StrategyNode[] children, Prediction predict, Player player)
        {
            if (player.CardsRemaining == 10)
                return 9;
            int lowestPlayed = 9;
            bool filledIn = false;
            for (int i = 0; i < Game.NUM_CARDS; i++)
            {
                if (player.Cards[i] == Card.Played)
                {
                    lowestPlayed = i;
                    if (i == 9 || player.Cards[i + 1] == Card.Played)
                        filledIn = true;
                    break;
                }
            }
            return filledIn ? (lowestPlayed == 1 ? 0 : lowestPlayed - 2) : lowestPlayed + 1;
        }

        public static int Bait(StrategyNode[] children, Prediction predict, Player player)
        {
            int ideal;
            if (predict.Certainty > 0.7)
                ideal = predict.Value + 1;
            else
                ideal = r.Next(10);

            int nearest = -1;
            for (int i = Game.NUM_CARDS - 1; i >= ideal; i--)
                if (player.Cards[i] == Card.Held)
                    nearest = i;

            if (nearest >= ideal)
                return nearest;

            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (player.Cards[i] == Card.Held)
                    return i;
            throw new InvalidOperationException("No cards available to play");
        }

        private static int NearestTo(int goal, Player player)
        {
            int upGoal = goal, downGoal = goal;
            while (player.CardsRemaining > 0)
            {
                if (upGoal < 10 && upGoal >= 0 && player.Cards[upGoal] == Card.Held)
                    return upGoal;
                if (downGoal >= 0 && downGoal < 10 && player.Cards[downGoal] == Card.Held)
                    return downGoal;
                upGoal++;
                downGoal--;
            }
            throw new InvalidOperationException("No cards available to play");
        }
    }
}
