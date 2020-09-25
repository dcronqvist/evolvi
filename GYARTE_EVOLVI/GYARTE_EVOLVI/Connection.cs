using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYARTE_EVOLVI
{
    public class Connection
    {
        public double Weight { get; set; }

        public Connection()
        {
            //Weight = ((new Random()).NextDouble() * 2f) - 1f;

            Random RNG = new Random();

            double r = RNG.NextDouble();

            if(((r < 0.2)))
                Weight = (((RNG.NextDouble()) * 1f) - 0.5f);     


        }

        public Connection Crossbreed(Connection c)
        {
            Random RNG = new Random();

            Connection conn = new Connection();

            if (RNG.NextDouble() < 0.5)
            {
                conn.Weight = Weight;
            }
            else
            {
                conn.Weight = c.Weight;
            }

            return conn;
        }

        public Connection Clone()
        {
            Connection c = new Connection();
            c.Weight = Weight;

            return c;
        }
    }
}
