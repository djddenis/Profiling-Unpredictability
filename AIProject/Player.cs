using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProject
{
    public enum Card
    {
        Held,
        Played
    }

    public class Player
    {
        public Player Opponent { get; set; }

        public StrategyNode Strat { get; private set; }

        public int GamesPlayed { get; private set; }
        public int GamesWon { get; private set; }

        public int Score = 0;
        public Card[] Cards = new Card[Game.NUM_CARDS];
        public int CardsRemaining { get { return Cards.Where(i => i == Card.Held).Count(); } }

        public int TotalPlays { get; private set; }
        public int[][] WhileHoldingHistory { get { return whileHoldingHistory; } }
        private int[][] whileHoldingHistory;
        public int[][] WhileOppHoldingHistory { get { return whileOppHoldingHistory; } }
        private int[][] whileOppHoldingHistory;

        public Player(StrategyNode strat)
        {
            TotalPlays = 0;
            InitializeHistory(ref whileHoldingHistory);
            InitializeHistory(ref whileOppHoldingHistory);
            InitializeCards();
            Opponent = null;
            Strat = strat;
        }

        public void ClearHistory()
        {
            TotalPlays = 0;
            InitializeHistory(ref whileHoldingHistory);
            InitializeHistory(ref whileOppHoldingHistory);
        }

        private void InitializeCards()
        {
            for (int i = 0; i < Game.NUM_CARDS; i++)
                Cards[i] = Card.Held;
        }

        private void InitializeHistory(ref int[][] toSetup)
        {
            toSetup = new int[Game.NUM_CARDS][];
            for (int i = 0; i < Game.NUM_CARDS; i++)
            {
                toSetup[i] = new int[Game.NUM_CARDS];
                for (int j = 0; j < Game.NUM_CARDS; j++)
                    toSetup[i][j] = 0;
            }
        }

        public void PlayCard(int choice)
        {
            if (Cards[choice] != Card.Held)
                throw new InvalidOperationException("Cannot play card that is not held");
            
            TotalPlays++;

            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (Cards[i] == Card.Held)
                    WhileHoldingHistory[choice][i]++;

            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (Opponent.Cards[i] == Card.Held)
                    WhileOppHoldingHistory[choice][i]++;

            Cards[choice] = Card.Played;
        }

        public int ChooseCard()
        {
            return Strat.GetChoice(MakePrediction(), this);
        }

        public Prediction MakePrediction()
        {
            int mostLikely = 0;
            double certainty = 0;
            for (int i = 0; i < Game.NUM_CARDS; i++)
            {
                double chance = CalculatePrediction(i);
                if (chance > certainty)
                {
                    certainty = chance;
                    mostLikely = i;
                }
            }
            return new Prediction(mostLikely, certainty);
        }

        private double CalculatePrediction(int num)
        {
            if (Opponent.Cards[num] != Card.Held)
                return 0;

            int scoreForMyHoldings = 0;
            for (int i = 0; i < Game.NUM_CARDS; i++)
                if(Cards[i] == Card.Held)
                    scoreForMyHoldings += Opponent.WhileOppHoldingHistory[num][i];

            int scoreForTheirHoldings = 0;
            for (int i = 0; i < Game.NUM_CARDS; i++)
                if (Opponent.Cards[i] == Card.Held)
                    scoreForTheirHoldings += Opponent.WhileHoldingHistory[num][i];

            double chanceByMyHoldings = (double)scoreForMyHoldings / (((double)Opponent.TotalPlays / 10.0) * (double)Opponent.CardsRemaining);
            double chanceByTheirHoldings = (double)scoreForTheirHoldings / (((double)Opponent.TotalPlays / 10.0) * (double)Opponent.CardsRemaining);
            return (chanceByMyHoldings + chanceByTheirHoldings) / 2.0;
        }

        public void FinishGame(bool won)
        {
            InitializeCards();
            Score = 0;
            GamesPlayed++;
            Opponent = null;
            if (won)
                GamesWon++;
        }
    }
}
