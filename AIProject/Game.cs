using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProject
{
    public class Game
    {
        public static readonly int NUM_CARDS = 10;

        public Player PlayerOne { get; set; }
        public Player PlayerTwo { get; set; }

        public Game(Player one, Player two)
        {
            PlayerOne = one;
            PlayerTwo = two;
        }

        public Player Play()
        {
            PlayerOne.Opponent = PlayerTwo;
            PlayerTwo.Opponent = PlayerOne;
            for (int i = 0; i < NUM_CARDS; i++)
            {
                int oneChoice = PlayerOne.ChooseCard();
                int twoChoice = PlayerTwo.ChooseCard();
                PlayerOne.PlayCard(oneChoice);
                PlayerTwo.PlayCard(twoChoice);
                if (oneChoice > twoChoice)
                    PlayerOne.Score++;
                if (twoChoice > oneChoice)
                    PlayerTwo.Score++;
            }
            PlayerOne.FinishGame(PlayerOne.Score > PlayerTwo.Score);
            PlayerTwo.FinishGame(PlayerTwo.Score > PlayerOne.Score);
            if (PlayerOne.Score > PlayerTwo.Score)
                return PlayerOne;
            if (PlayerTwo.Score > PlayerOne.Score)
                return PlayerTwo;
            return null;
        }

        public Player PlayObserved()
        {
            PlayerOne.Opponent = PlayerTwo;
            PlayerTwo.Opponent = PlayerOne;
            for (int i = 0; i < NUM_CARDS; i++)
            {
                Prediction onePred = PlayerOne.MakePrediction();
                Prediction twoPred = PlayerTwo.MakePrediction();

                int oneChoice = PlayerOne.Strat.GetChoice(onePred, PlayerOne);
                int twoChoice = PlayerTwo.Strat.GetChoice(twoPred, PlayerTwo);
                PlayerOne.PlayCard(oneChoice);
                PlayerTwo.PlayCard(twoChoice);
                if (oneChoice > twoChoice)
                    PlayerOne.Score++;
                if (twoChoice > oneChoice)
                    PlayerTwo.Score++;
                Console.WriteLine("P1 Prediction: (" + onePred.Value + ", " + onePred.Certainty + "), P1 Choice: " + oneChoice);
                Console.WriteLine("P2 Prediction: (" + twoPred.Value + ", " + twoPred.Certainty + "), P2 Choice: " + twoChoice);
                Console.WriteLine("Score: " + PlayerOne.Score + " to " + PlayerTwo.Score);
                Console.ReadLine();
            }
            Console.WriteLine("Game finished.  Winner is " + ((PlayerOne.Score > PlayerTwo.Score) ? "P1" : ((PlayerTwo.Score > PlayerOne.Score) ? "P2" : "tied")));
            Console.ReadLine();
            PlayerOne.FinishGame(PlayerOne.Score > PlayerTwo.Score);
            PlayerTwo.FinishGame(PlayerTwo.Score > PlayerOne.Score);
            if (PlayerOne.Score > PlayerTwo.Score)
                return PlayerOne;
            if (PlayerTwo.Score > PlayerOne.Score)
                return PlayerTwo;
            return null;
        }
    }
}
