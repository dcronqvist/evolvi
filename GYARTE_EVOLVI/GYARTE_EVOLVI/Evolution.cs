using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySQLAPI;
using Newtonsoft.Json;
using System.IO;

namespace GYARTE_EVOLVI
{
    public class Evolution
    {
        public string TimeStarted { get; set; }
        public string TimeStopped { get; set; }

        public List<Generation> Generations { get; set; }

        public Evolution()
        {
            Generations = new List<Generation>();
        }

        public void Start()
        {
            TimeStarted = DateTime.Now.ToShortTimeString();           
        }

        public void Stop(string path)
        {
            TimeStopped = DateTime.Now.ToShortTimeString();

            Network n = GeneticAlgorithm.SortFittest().Last().Network;

            n.SaveToFile(path);
        }

        public void AddGeneration(Generation g)
        {
            Generations.Add(g);
        }

        //public Evolution(int id)
        //{
        //    Generations = new List<Generation>();

        //    string[] generationFiles = Directory.GetFiles(@"C:\Users\daniel.cronqvist\Dropbox\Evolutions\evo_" + id);

        //    foreach(string file in generationFiles)
        //    {
        //        string textInFile = File.ReadAllText(file);

        //        JsonSerializerSettings sett = new JsonSerializerSettings();

        //        sett.Formatting = Formatting.Indented;

        //        Generation g = JsonConvert.DeserializeObject<Generation>(textInFile, sett);

        //        Generations.Add(g);
        //    }
        //}
    }

    public class Generation
    {
        public Evolvi[] Evolvis { get; set; }
        public Evolvi Fittest
        {
            get
            {
                return Evolvis.ToList().OrderBy(x => x.Energy).ToList().Last();
            }
        }
        public float AverageFitness
        {
            get
            {
                float total = 0;

                foreach(Evolvi e in Evolvis)
                {
                    total += e.Energy;
                }

                return (total / Evolvis.Length);
            }
        }
        public float MedianFitness
        {
            get
            {
                if (Evolvis.Length % 2 == 1)
                {
                    int amount = Evolvis.Length;
                    int middle = (amount / 2) + 1;

                    return Evolvis[middle].Energy;
                }
                else
                {
                    int amount = Evolvis.Length;
                    int middleDown = (amount / 2);
                    int middleUp = middleDown + 1;

                    float average = (Evolvis[middleDown].Energy + Evolvis[middleUp].Energy) / 2;

                    return average;
                }
            }
        }
        public float TopFitness
        {
            get
            {
                float highest = 0;

                for (int i = 0; i < Evolvis.Count(); i++)
                {
                    if(Evolvis[i].Energy > highest)
                    {
                        highest = Evolvis[i].Energy;
                    }
                }

                return highest;
            }
        }
        public float LowestFitness
        {
            get
            {
                float lowest = TopFitness;

                for (int i = 0; i < Evolvis.Count(); i++)
                {
                    if(Evolvis[i].FoodEaten < lowest)
                    {
                        lowest = Evolvis[i].FoodEaten;
                    }
                }

                return lowest;
            }
        }

        public Generation(List<Evolvi> evolvis)
        {
            Evolvis = new Evolvi[evolvis.Count];
            evolvis.CopyTo(Evolvis);
        }

        public List<Evolvi> CloneEvolvis()
        {
            List<Evolvi> lst = new List<Evolvi>();

            foreach(Evolvi e in Evolvis)
            {
                lst.Add(e.CloneAll());
            }

            return lst;
        }
    }
}
