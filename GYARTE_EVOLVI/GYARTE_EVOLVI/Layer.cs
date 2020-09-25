using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYARTE_EVOLVI
{
    public class Layer
    {
        public List<Neuron> Neurons { get; set; }
        public int NeuronCount { get { return Neurons.Count; } }

        public Layer()
        {

        }

        public Layer(int numNeurons)
        {
            Neurons = new List<Neuron>(numNeurons);
        }

        public Layer Crossbreed(Layer lastLayer, Layer l)
        {
            Random RNG = new Random();

            Layer newLayer = new Layer(NeuronCount);

            for (int i = 0; i < NeuronCount; i++)
            {
                newLayer.Neurons.Add(Neurons[i].Crossbreed(l.Neurons[i]));
            }

            return newLayer;
        }

        public Layer Clone()
        {
            Layer l = new Layer(NeuronCount);

            foreach(Neuron n in Neurons)
            {
                l.Neurons.Add(n.Clone());
            }

            return l;
        }
    }
}
