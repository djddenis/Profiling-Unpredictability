using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIProject
{
    public class Prediction
    {
        public int Value { get; private set; }
        public double Certainty { get; private set; }

        public Prediction(int value, double certainty)
        {
            Value = value;
            Certainty = certainty;
        }
    }
}
