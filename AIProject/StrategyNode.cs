using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProject
{
    public delegate int Strategy(StrategyNode[] children, Prediction predict, Player me);
    public class StrategyNode
    {
        private StrategyNode[] Children;
        private Strategy Strat;

        public StrategyNode(Strategy strat, StrategyNode[] children)
        {
            Strat = strat;
            Children = children; 
        }

        public int GetChoice(Prediction predict, Player me)
        {
            return Strat(Children, predict, me);
        }
    }
}
