using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYARTE_EVOLVI
{
    public class Neuron
    {
        public List<Connection> Connections { get; set; }
        public double Bias { get; set; }
        public double Delta { get; set; }
        public double Value { get; set; }

        public int ConnectionCount { get { return Connections.Count; } }

        public Neuron()
        {
            Random r = new Random();

            if(r.NextDouble() < 0.1)
                Bias = ((r.NextDouble()) * 0.125) - 0.0625;
            else
                Bias = 0;

            Connections = new List<Connection>();
        }

        public Neuron Crossbreed(Neuron n)
        {
            Neuron newN = new Neuron();

            // CONNECTIONS

            Random RNG = new Random();

            for (int i = 0; i < ConnectionCount; i++)
            {
                newN.Connections.Add(Connections[i].Crossbreed(n.Connections[i]));
            }

            // BIAS
            if(RNG.NextDouble() < 0.5)
            {
                newN.Bias = Bias;
            }
            else
            {
                newN.Bias = n.Bias;
            }

            return newN;
        }

        public Neuron Clone()
        {
            Neuron n = new Neuron();
            n.Bias = Bias;
            n.Delta = Delta;
            n.Value = Value;

            foreach(Connection c in Connections)
            {
                n.Connections.Add(c.Clone());
            }

            return n;
        }
    }
}
