using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using GEPSharp;

namespace AIProject
{
    class Program
    {
        static Problem Problem;
        static int PopSize = 10;
        static int NumGenerations = 15;
        static int NumGames = 100;
        static int HeadLength = 14;
        static bool TestVsStatics = true;
        static int NumStaticStrats;

        static int CurrentGeneration = 0;

        static Game Game;
        static Player[] Players;

        static void Main(string[] args)
        {
            List<Node> nodes = new List<Node>();
            AddFunctionsAndTerminals(nodes);
            
            Game = new Game(null, null);
            Players = new Player[PopSize];
            Problem = new GEPProblem(PopSize, SimpleFitness, HeadLength, nodes);
            Problem.EvaluateRound();

            Console.WriteLine("This test requires " + CalcNumGamesForRun() + " games to complete.");

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < NumGenerations; i++)
                RunGeneration();
            watch.Stop();

            Console.WriteLine("Finished test in " + watch.Elapsed + ".");
            Console.WriteLine("Best after " + NumGenerations + " generations: (" + Problem.SolutionFitness + " fitness)");
            Console.WriteLine(((GEPIndividual)Problem.BestSoFar).ToGraphVis());
            Console.ReadLine();

            Console.WriteLine("Best vs Optimal");
            Player best = new Player((StrategyNode)Problem.Solution);
            Player optimelle = new Player(new StrategyNode(StaticStrategies.Optimal, null));
            Player human = new Player(new StrategyNode(StaticStrategies.PlayerChoice, null));
            PlayGames(best, optimelle, NumGames);
            Console.WriteLine("Best won " + best.GamesWon + " games");
            Game.PlayObserved();
            Game.PlayerTwo = human;
            Game.PlayObserved();
        }

        static int CalcNumGamesForRun()
        {
            int interactionsPerGeneration = (PopSize * (PopSize - 1)) / 2 + PopSize * NumStaticStrats;
            int gamesPerGeneration = interactionsPerGeneration * NumGames;
            return NumGenerations * gamesPerGeneration;
        }

        static void RunGeneration()
        {
            CurrentGeneration++;
            Problem.EvaluateRounds(1);
            
            for (int i = 0; i < PopSize; i++)
                Players[i] = new Player((StrategyNode)Problem.Individuals[i].Answer);

            for (int i = 0; i < PopSize; i++)
            {
                for (int j = i + 1; j < PopSize; j++)
                {
                    PlayGames(Players[i], Players[j], NumGames);
                    Players[i].ClearHistory();
                    Players[j].ClearHistory();
                }
            }

            Problem.EvaluateRound();
            CreateGvFile((GEPIndividual)Problem.BestSoFar);
        }

        static void CreateGvFile(GEPIndividual ind)
        {
            if(!Directory.Exists("GraphVis"))
                Directory.CreateDirectory("GraphVis");
            using (TextWriter writer = new StreamWriter(Path.Combine("GraphVis", "BestAfter" + CurrentGeneration + ".gv")))
            {
                writer.Write(ind.ToGraphVis());
            }
        }

        static double SimpleFitness(object obj)
        {
            StrategyNode strat = obj as StrategyNode;

            int myNum = -1;
            for (int i = 0; i < PopSize; i++)
            {
                if (Problem.Individuals[i].Answer == strat)
                {
                    myNum = i;
                    break;
                }
            }

            Player me = Players[myNum];
            double pvpScore = me == null ? 0 : (double)me.GamesWon / (double)me.GamesPlayed;

            if (TestVsStatics)
            {
                PlayVsStatics(me);
                pvpScore += me == null ? 0 : (double)me.GamesWon / (double)me.GamesPlayed;
            }

            return pvpScore;
        }

        private static void PlayVsStatics(Player me)
        {
            List<Player> staticStrats = new List<Player>();
            staticStrats.Add(new Player(new StrategyNode(StaticStrategies.Optimal, null)));
            staticStrats.Add(new Player(new StrategyNode(StaticStrategies.Bait, null)));
            staticStrats.Add(new Player(new StrategyNode(StaticStrategies.HighestFirst, null)));
            staticStrats.Add(new Player(new StrategyNode(StaticStrategies.LowestFirst, null)));
            staticStrats.Add(new Player(new StrategyNode(StaticStrategies.RandomWidening, null)));
            staticStrats.Add(new Player(new StrategyNode(StaticStrategies.SkipFromHigh, null)));
            staticStrats.Add(new Player(new StrategyNode(StaticStrategies.SkipFromLow, null)));
            NumStaticStrats = staticStrats.Count;
            if (me != null)
            {
                foreach (Player statPlayer in staticStrats)
                {
                    me.ClearHistory();
                    PlayGames(me, statPlayer, NumGames);
                }
            }
        }

        private static void PlayGames(Player one, Player two, int numGames)
        {
            Game.PlayerOne = one;
            Game.PlayerTwo = two;
            for (int i = 0; i < numGames; i++)
                Game.Play();
        }

        private static void AddFunctionsAndTerminals(List<Node> nodes)
        {
            nodes.Add(new Node("MYHIGH", new StrategyNode(FunctionsAndTerminals.MyHighest, null)));
            nodes.Add(new Node("MYLOW", new StrategyNode(FunctionsAndTerminals.MyLowest, null)));
            nodes.Add(new Node("MYCLOSE", new StrategyNode(FunctionsAndTerminals.MyClosest, null)));

            nodes.Add(new Node("COIN", 2, (i => new StrategyNode(FunctionsAndTerminals.CoinFlip, new StrategyNode[] { (StrategyNode)i[0], (StrategyNode)i[1] }))));
            nodes.Add(new Node("HIGHWIN", 2, (i => new StrategyNode(FunctionsAndTerminals.HighestWins, new StrategyNode[] { (StrategyNode)i[0], (StrategyNode)i[1] }))));
            nodes.Add(new Node("CLOSEWIN", 2, (i => new StrategyNode(FunctionsAndTerminals.ClosestWins, new StrategyNode[] { (StrategyNode)i[0], (StrategyNode)i[1] }))));
            nodes.Add(new Node("CERTAIN", 2, (i => new StrategyNode(FunctionsAndTerminals.HighCertainty, new StrategyNode[] { (StrategyNode)i[0], (StrategyNode)i[1] }))));

            nodes.Add(new Node("PLUS", 1, (i => new StrategyNode(FunctionsAndTerminals.PlusOne, new StrategyNode[] { (StrategyNode)i[0] }))));
            nodes.Add(new Node("MINUS", 1, (i => new StrategyNode(FunctionsAndTerminals.MinusOne, new StrategyNode[] { (StrategyNode)i[0] }))));
        }
    }
}
