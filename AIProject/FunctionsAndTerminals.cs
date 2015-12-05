using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProject
{
    public static class FunctionsAndTerminals
    {
        static Random r = new Random();

        public static int MyHighest(StrategyNode[] children, Prediction predict, Player me)
        {
            for (int i = Game.NUM_CARDS - 1; i >= 0; i--)
                if (me.Cards[i] == Card.Held)
                    return i;
            throw new InvalidOperationException("No cards available to play");
        }

        public static int MyLowest(StrategyNode[] children, Prediction predict, Player me)
        {
            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (me.Cards[i] == Card.Held)
                    return i;
            throw new InvalidOperationException("No cards available to play");
        }

        public static int MyClosest(StrategyNode[] children, Prediction predict, Player me)
        {
            return ClosestTo(predict.Value + 1, me);
        }

        private static int ClosestTo(int ideal, Player me)
        {
            ideal = ideal > 9 ? 9 : (ideal < 0 ? 0 : ideal);
            int nearest = -1;
            for (int i = Game.NUM_CARDS - 1; i >= ideal; i--)
                if (me.Cards[i] == Card.Held)
                    nearest = i;

            if (nearest >= ideal)
                return nearest;

            for (int i = ideal - 1; i >= 0; i--)
                if (me.Cards[i] == Card.Held)
                    return i;
            throw new InvalidOperationException("No cards available to play");
        }

        public static int CoinFlip(StrategyNode[] children, Prediction predict, Player me)
        {
            if (r.Next(0, 2) == 0)
                return children[0].GetChoice(predict, me);
            return children[1].GetChoice(predict, me);
        }

        public static int PlusOne(StrategyNode[] children, Prediction predict, Player me)
        {
            return ClosestTo(children[0].GetChoice(predict, me) + 1, me);
        }

        public static int MinusOne(StrategyNode[] children, Prediction predict, Player me)
        {
            return ClosestTo(children[0].GetChoice(predict, me) - 1, me);
        }

        public static int HighestWins(StrategyNode[] children, Prediction predict, Player me)
        {
            if (MyHighest(null, predict, me) > predict.Value)
                return children[0].GetChoice(predict, me);
            return children[1].GetChoice(predict, me);
        }

        public static int ClosestWins(StrategyNode[] children, Prediction predict, Player me)
        {
            if (MyClosest(null, predict, me) > predict.Value)
                return children[0].GetChoice(predict, me);
            return children[1].GetChoice(predict, me);
        }

        public static int HighCertainty(StrategyNode[] children, Prediction predict, Player me)
        {
            if (predict.Certainty >= 0.7)
                return children[0].GetChoice(predict, me);
            return children[1].GetChoice(predict, me);
        }
    }
}
