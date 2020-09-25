using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace GYARTE_EVOLVI
{
    public class Network
    {
        public List<Layer> Layers { get; set; }
        public int LayerCount { get { return Layers.Count; } }

        public Network()
        {

        }

        public static Network LoadFromFile(string path)
        {
            Network n = JsonConvert.DeserializeObject<Network>(File.ReadAllText(path));

            return n;
        }

        public void SaveToFile(string path)
        {
            string text = JsonConvert.SerializeObject(this, Formatting.Indented);

            using(StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.CreateNew), Encoding.Default))
            {
                sw.Write(text);
            }
        }

        public Network(int[] layers)
        {
            if (layers.Length < 2) return;

            Layers = new List<Layer>();

            int thingsToDo = 0;

            for (int i = 0; i < layers.Length; i++)
            {
                thingsToDo += (layers[i]);
            }

            float loadAmount = 1f / GeneticAlgorithm.evolviAmount;
            loadAmount /= thingsToDo;

            for (int i = 0; i < layers.Length; i++)
            {
                Layer layer = new Layer(layers[i]);
                Layers.Add(layer);

                for (int j = 0; j < layers[i]; j++)
                {
                    layer.Neurons.Add(new Neuron());

                    Thread.Sleep(10);
                }

                layer.Neurons.ForEach(x =>
                {
                    //if (i == LayerCount - 1) x.Bias = 0;

                    if (i == 0)
                    {
                        x.Bias = 0;
                        Thread.Sleep(10);
                    }
                    else
                        for (int k = 0; k < layers[i - 1]; k++)
                        {
                            x.Connections.Add(new Connection());

                            Thread.Sleep(10);
                        }

                    if (GeneticAlgorithm.Loading)
                    {
                        GeneticAlgorithm.EvolviLoadedAmount += loadAmount;
                    }
                });
            }
        }

        public Network(bool s)
        {
            Layers = new List<Layer>();
        }

        private double Sigmond(double x)
        {
            //return (1 / (1 + Math.Pow(Math.E, -x)));

            //if (x < 0)
            //{
            //    return (Math.Pow(Math.E, x) - 1);
            //}
            //else
            //{
            //    return x;
            //}

            return Math.Atan(x);
        }

        public double[] Run(List<double> input)
        {
            if (input.Count != Layers[0].NeuronCount) return null;

            for (int i = 0; i < LayerCount; i++)
            {
                Layer layer = Layers[i];

                for (int j = 0; j < layer.NeuronCount; j++)
                {
                    Neuron neuron = layer.Neurons[j];

                    if (i == 0)
                        neuron.Value = input[j];
                    else
                    {
                        neuron.Value = 0;

                        for (int n = 0; n < Layers[i - 1].NeuronCount; n++)
                        {
                            neuron.Value = neuron.Value + Layers[i - 1].Neurons[n].Value * neuron.Connections[n].Weight;

                            if(n != 0 || n != LayerCount - 1)
                                neuron.Value = Sigmond(neuron.Value + neuron.Bias);
                        }
                    }
                }          
            }

            Layer last = Layers.Last();
            int numOutput = last.NeuronCount;
            double[] output = new double[numOutput];
            for (int i = 0; i < last.NeuronCount; i++)
            {
                output[i] = last.Neurons[i].Value;
            }

            return output;
        }

        public Network Crossbreed(Network n)
        {
            Network newN = new Network(true);

            for (int i = 0; i < LayerCount; i++)
            {
                newN.Layers.Add(Layers[i].Crossbreed(((i - 1) > -1 ? (n.Layers[i - 1]) : null), n.Layers[i]));
            }

            return newN;
        }

        public Network Clone()
        {
            Network n = new Network(true);

            foreach(Layer l in Layers)
            {
                n.Layers.Add(l.Clone());
            }

            return n;
        }

    }
}
