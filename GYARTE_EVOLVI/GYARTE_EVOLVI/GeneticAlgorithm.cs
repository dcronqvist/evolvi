using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using MySQLAPI;
using System.Diagnostics;

namespace GYARTE_EVOLVI
{
    public static class GeneticAlgorithm
    {
        //LOADING VARIABLES
        public static float EvolviMaxAmount = 1f;
        public static float EvolviLoadedAmount = 0f;
        public static bool Loading = false;

        public static int evolviAmount = 25;
        public static int foodAmount = 40;

        public static List<Evolvi> CurrentGeneration { get; set; }
        public static List<Food> CurrentFood { get; set; }

        public static BoundingBox WallLeft;
        public static BoundingBox WallTop;
        public static BoundingBox WallRight;
        public static BoundingBox WallBottom;

        public static int GenerationTime { get; set; }
        public static float ElapsedGenerationTime { get; set; }
        public static int EvolutionSpeed { get; set; }
        public static int Generation { get; set; }

        public static float mutationRate = 0.05f;

        public static Evolution thisEvolution;

        public static event EventHandler NewGeneration;

        public struct DistanceEvaluator
        {
            public float Distance { get; set; }
            public Vector2 End { get; set; }

            public DistanceEvaluator(float dist, Vector2 end)
            {
                Distance = dist;
                End = end;
            }
        }

        static GeneticAlgorithm()
        {
            WallLeft = new BoundingBox(new Vector3(-1, 0, 0), new Vector3(0, 2000, 0));
            WallTop = new BoundingBox(new Vector3(0, -1, 0), new Vector3(2000, 0, 0));
            WallRight = new BoundingBox(new Vector3(2000, 0, 0), new Vector3(2001, 2000, 0));
            WallBottom = new BoundingBox(new Vector3(0, 2000, 0), new Vector3(2000, 2001, 0));


            CurrentGeneration = new List<Evolvi>();
            CurrentFood = new List<Food>();
            GenerationTime = 180;
            EvolutionSpeed = 0;
            Generation = 0;
            ElapsedGenerationTime = 0;
        }

        static private void OnNewGeneration(object sender, EventArgs e)
        {
            NewGeneration?.Invoke(sender, e);
        }

        public static void Initialize()
        {
            thisEvolution = new Evolution();
            thisEvolution.Start();

            Loading = true;


            Random RNG = new Random();

            EvolviMaxAmount = 1f;
            EvolviLoadedAmount = 0;

            CurrentGeneration.Clear();

            for (int i = 0; i < evolviAmount; i++)
            {
                CurrentGeneration.Add(new Evolvi(new Vector2(RNG.Next(20, 2000 - 20), RNG.Next(20, 2000 - 20))));
            }

            CurrentFood.Clear();

            for (int i = 0; i < foodAmount; i++)
            {
                CurrentFood.Add(new Food(new Vector2(RNG.Next(20, 2000 - 20), RNG.Next(20, 2000 - 20))));

                Thread.Sleep(10);
            }

            Loading = false;
        }

        public static bool AnyoneAlive()
        {
            foreach(Evolvi e in CurrentGeneration)
            {
                if (!e.Dead)
                    return true;
            }

            return false;
        }

        public static void Initialize(bool overrideDefaults)
        {
            thisEvolution = new Evolution();
            thisEvolution.Start();

            Loading = true;



            Random RNG = new Random();

            EvolviMaxAmount = 1f;
            EvolviLoadedAmount = 0;

            CurrentGeneration.Clear();

            if (overrideDefaults == true)
            {

                for (int i = 0; i < evolviAmount; i++)
                {
                    CurrentGeneration.Add(new Evolvi(new Vector2(RNG.Next(20, 2000 - 20), RNG.Next(20, 2000 - 20))));
                }
            }

            CurrentFood.Clear();

            for (int i = 0; i < foodAmount; i++)
            {
                CurrentFood.Add(new Food(new Vector2(RNG.Next(20, 2000 - 20), RNG.Next(20, 2000 - 20))));

                Thread.Sleep(10);
            }

            Loading = false;
        }

        public static void Reset()
        {
            CurrentGeneration = new List<Evolvi>();
            CurrentFood = new List<Food>();
            GenerationTime = 180;
            EvolutionSpeed = 0;
            Generation = 0;
            ElapsedGenerationTime = 0;
        }

        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < EvolutionSpeed; i++)
            {
                DoUpdate(gameTime);
            }
        }

        public static List<Evolvi> SortFittest()
        {
            List<Evolvi> sorted = CurrentGeneration.OrderBy(x => x.Energy).ToList();

            return sorted;
        }

        public static void DoUpdate(GameTime gameTime)
        {
            if((ElapsedGenerationTime > GenerationTime && GenerationTime != 0) || !AnyoneAlive())
            {
                // GENETIC ALGORITHM THIS SHIT

                thisEvolution.AddGeneration(new Generation(CurrentGeneration));


                if (CurrentGeneration.Count > 0)
                {
                    List<Evolvi> sorted = SortFittest();

                    int root = (int)Math.Sqrt(sorted.Count);

                    sorted = sorted.OrderByDescending(x => x.Energy).ToList();

                    List<Evolvi> newGen = new List<Evolvi>();

                    while (newGen.Count < evolviAmount)
                    {
                        Evolvi e = sorted.First().Crossbreed(sorted.First());
                        e.Mutate(mutationRate);

                        newGen.Add(e);

                        Thread.Sleep(10);
                    }

                    CurrentGeneration.Clear();

                    CurrentGeneration = newGen;
                }
                else
                {
                    Random RNG = new Random();

                    Evolvi e = new Evolvi(new Vector2(RNG.Next(0, 2000), RNG.Next(0, 2000)));

                    for (int i = 0; i < evolviAmount; i++)
                    {
                        CurrentGeneration.Add(e.Clone());

                        Thread.Sleep(10);
                    }
                }


                ElapsedGenerationTime = 0;
                Generation++;


                OnNewGeneration(null, EventArgs.Empty);
            }

            ElapsedGenerationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            List<Evolvi> deadEvolvis = new List<Evolvi>();

            foreach(Evolvi evolvi in CurrentGeneration)
            {
                if (!evolvi.Dead)
                {
                    if (evolvi.Energy <= 0)
                    {
                        evolvi.Dead = true;
                    }

                    Input[] inpts = new Input[evolvi.AreasOfVision + 1];
                    int indexer = 0;

                    if (Math.Abs(evolvi.AngleDegrees) > 360)
                    {
                        evolvi.AngleDegrees = 0;
                    }

                    float anglePerAreaOfVision = (float)evolvi.FOV / (float)(evolvi.AreasOfVision + 1);

                    for (float i = -evolvi.FOV / 2f; i < (evolvi.FOV / 2f); i += anglePerAreaOfVision)
                    {
                        inpts[indexer] = new Input();

                        Vector2 start = evolvi.Position;
                        Vector2 end;

                        float xDiff = (float)Math.Cos(evolvi.AngleRadians + MathHelper.ToRadians(i)) * evolvi.VisionRange;
                        float yDiff = (float)Math.Sin(evolvi.AngleRadians + MathHelper.ToRadians(i)) * evolvi.VisionRange;

                        end = start + new Vector2(xDiff, yDiff);
                        inpts[indexer].value = 0;

                        Vector2 dir = new Vector2(xDiff, yDiff);
                        dir.Normalize();

                        Ray ray = new Ray(new Vector3(start, 0), new Vector3(dir, 0));

                        float angle = (float)Math.Atan2(dir.Y, dir.X);

                        List<DistanceEvaluator> ends = new List<DistanceEvaluator>();

                        float intersectionToWall = RayIntersectsAWall(ray);

                        foreach (Food food in CurrentFood)
                        {
                            if (evolvi.CollisionBox.Intersects(food.CollisionBox))
                            {
                                evolvi.Energy += food.Nutrition;
                                evolvi.FoodEaten++;

                                Random RNG = new Random();

                                food.Position = new Vector2(RNG.Next(40, 2000 - 80), RNG.Next(40, 2000 - 80));
                                //food.CollisionBox = new Rectangle(food.Position.ToPoint(), food.TextureFood.Bounds.Size);
                                //food.BoundingBox = new BoundingBox(new Vector3(food.Position, 0), new Vector3(food.Position + food.CollisionBox.Size.ToVector2(), 0));

                                bool done = false;

                                while (!done)
                                {
                                    bool intersects = false;

                                    foreach (Food secondFood in CurrentFood)
                                    {
                                        if (food != secondFood)
                                        {
                                            if (food.CollisionBox.Intersects(secondFood.CollisionBox))
                                            {
                                                intersects = true;
                                            }
                                        }
                                    }

                                    if (!intersects)
                                    {
                                        done = true;
                                    }
                                    else
                                    {
                                        food.Position = new Vector2(RNG.Next(20, 2000 - 20), RNG.Next(20, 2000 - 20));
                                        //food.CollisionBox = new Rectangle(food.Position.ToPoint(), food.TextureFood.Bounds.Size);
                                        //food.BoundingBox = new BoundingBox(new Vector3(food.Position, 0), new Vector3(food.Position + food.CollisionBox.Size.ToVector2(), 0));
                                    }
                                }
                            }

                            if (ray.Intersects(food.BoundingBox) != 0)
                            {
                                if (ray.Intersects(food.BoundingBox) < intersectionToWall)
                                {
                                    if (ray.Intersects(food.BoundingBox) < evolvi.VisionRange)
                                    {
                                        float? dist = ray.Intersects(food.BoundingBox);

                                        ends.Add(new DistanceEvaluator((1f - (dist.Value / evolvi.VisionRange)) * (food.Nutrition / 50f), new Vector2((float)(Math.Cos(angle) * dist), (float)(Math.Sin(angle) * dist)) + start));
                                    }
                                }
                                else
                                {
                                    if (intersectionToWall < evolvi.VisionRange)
                                    {
                                        float? dist = intersectionToWall;

                                        ends.Add(new DistanceEvaluator(-(1 - (dist.Value / evolvi.VisionRange)), new Vector2((float)(Math.Cos(angle) * dist), (float)(Math.Sin(angle) * dist)) + start));
                                    }
                                }
                            }
                        }

                        if (ends.Count > 0)
                        {
                            List<DistanceEvaluator> sortedByDist = ends.OrderBy(x => x.Distance).ToList();

                            end = sortedByDist.Last().End;
                            inpts[indexer].value = sortedByDist.Last().Distance;

                        }

                        inpts[indexer].start = start;
                        inpts[indexer].end = end;

                        indexer++;
                    }

                    evolvi.Update(inpts);
                }
            }
        }

        public static float RayIntersectsAWall(Ray r)
        {
            float val = 0f;

            float? a;

            if((a = r.Intersects(WallTop)) != 0)
            {
                if (a.HasValue)
                    val = a.Value;
            }

            if((a = r.Intersects(WallRight)) != 0)
            {
                if (a.HasValue)
                    val = a.Value;
            }

            if((a = r.Intersects(WallLeft)) != 0)
            {
                if (a.HasValue)
                    val = a.Value;
            }

            if((a = r.Intersects(WallBottom)) != 0)
            {
                if (a.HasValue)
                    val = a.Value;
            }

            return val;
        }

        public static void Draw()
        {
            foreach(Evolvi evolvi in CurrentGeneration)
            {
                if(!evolvi.Dead)
                    evolvi.Draw();
            }

            foreach(Food food in CurrentFood)
            {
                food.Draw();
            }
        }
    }
}
