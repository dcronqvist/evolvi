using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYARTE_EVOLVI
{
    public static class MyGrapher
    {
        public enum Grapher
        {
            Median,
            Average,
            Top,
            Lowest
        }

        public static Graph graph;
        public static Grapher graphType;

        static Button btn_close;

        static Button btn_median;
        static Button btn_average;
        static Button btn_highest;
        static Button btn_lowest;

        public static bool IsActive { get; set; }
        public static int XScale { get; set; }

        public static Vector2 GrapherWindowPosition { get; set; }
        public static Vector2 GrapherWindowSize { get; set; }

        static Color buttonPressed = Color.DarkBlue;
        static Color buttonReleased = Color.LightSkyBlue;

        static int gen;
        static List<Evolvi> saved;

        static float timeHovered;
        static float timeToHover = 2f;

        static MyGrapher()
        {
            XScale = 1;
            gen = -1;
            IsActive = false;
            GrapherWindowPosition = new Vector2(200, 200);
            GrapherWindowSize = new Vector2(650, 650);

            btn_close = new Button(GrapherWindowPosition + new Vector2(10, 10), new Point(70, 30), "Close", Color.DarkGray);

            btn_close.Clicked += Btn_close_Clicked;

            graph = new Graph(GameHelper.GraphicsDevice, new Point((int)GrapherWindowSize.X - 40, (int)GrapherWindowSize.Y - 70));
            graph.Position = GrapherWindowPosition + new Vector2(20, GrapherWindowSize.Y - 40);

            graphType = Grapher.Average;

            btn_median = new Button(GrapherWindowPosition + new Vector2(90, 10), new Point(70, 30), "Median", buttonReleased);
            btn_average = new Button(GrapherWindowPosition + new Vector2(170, 10), new Point(70, 30), "Average", buttonPressed);
            btn_highest = new Button(GrapherWindowPosition + new Vector2(250, 10), new Point(70, 30), "Highest", buttonReleased);
            btn_lowest = new Button(GrapherWindowPosition + new Vector2(330, 10), new Point(70, 30), "Lowest", buttonReleased);

            btn_median.Clicked += Btn_median_Clicked;
            btn_average.Clicked += Btn_average_Clicked;
            btn_highest.Clicked += Btn_highest_Clicked;
            btn_lowest.Clicked += Btn_lowest_Clicked;
        }

        private static void Btn_lowest_Clicked(object sender, EventArgs e)
        {
            ResetButtons();
            btn_lowest.Color = buttonPressed;

            graphType = Grapher.Lowest;
        }

        private static void Btn_highest_Clicked(object sender, EventArgs e)
        {
            ResetButtons();
            btn_highest.Color = buttonPressed;

            graphType = Grapher.Top;
        }

        private static void Btn_average_Clicked(object sender, EventArgs e)
        {
            ResetButtons();
            btn_average.Color = buttonPressed;


            graphType = Grapher.Average;
        }

        private static void Btn_median_Clicked(object sender, EventArgs e)
        {
            ResetButtons();
            btn_median.Color = buttonPressed;


            graphType = Grapher.Median;
        }

        public static bool MouseWithinWindow()
        {
            bool within = false;

            Rectangle winRec = new Rectangle((int)GrapherWindowPosition.X + 20, (int)GrapherWindowPosition.Y + 50, (int)GrapherWindowSize.X - 40, (int)GrapherWindowSize.Y - 70);

            if (InputManager.MouseBoxScreen.Intersects(winRec))
            {
                within = true;
            }

            return within;
        }

        public static void ResetButtons()
        {
            btn_lowest.Color = buttonReleased;
            btn_highest.Color = buttonReleased;
            btn_median.Color = buttonReleased;
            btn_average.Color = buttonReleased;
        }

        private static void Btn_close_Clicked(object sender, EventArgs e)
        {
            IsActive = false;
        }

        public static void Update()
        {
            if (InputManager.PressingMouseLeft())
            {
                if (InputManager.MouseBoxScreen.Intersects(new Rectangle(GrapherWindowPosition.ToPoint(), GrapherWindowSize.ToPoint())))
                {
                    Vector2 offset = GrapherWindowPosition - InputManager.MousePositionScreen;

                    GrapherWindowPosition = InputManager.MousePositionScreen + offset;

                    GrapherWindowPosition += InputManager.MouseVelocityScreen;
                }
            }

            graph.Position = GrapherWindowPosition + new Vector2(20, GrapherWindowSize.Y - 20);

            btn_close.Position = GrapherWindowPosition + new Vector2(10, 10);
            btn_median.Position = GrapherWindowPosition + new Vector2(90, 10);
            btn_average.Position = GrapherWindowPosition + new Vector2(170, 10);
            btn_highest.Position = GrapherWindowPosition + new Vector2(250, 10);
            btn_lowest.Position = GrapherWindowPosition + new Vector2(330, 10);

            btn_close.Update();
            btn_median.Update();
            btn_average.Update();
            btn_highest.Update();
            btn_lowest.Update();
        }

        public static void Draw()
        {
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], GrapherWindowPosition, null, new Color(80, 80, 80, 200), 0f, Vector2.Zero, GrapherWindowSize, SpriteEffects.None, 0f);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], GrapherWindowPosition, null, Color.Black, 0f, Vector2.Zero, new Vector2(2, GrapherWindowSize.Y), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], GrapherWindowPosition, null, Color.Black, 0f, Vector2.Zero, new Vector2(GrapherWindowSize.X, 2), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], GrapherWindowPosition + new Vector2(GrapherWindowSize.X, 0), null, Color.Black, 0f, Vector2.Zero, new Vector2(2, GrapherWindowSize.Y), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], GrapherWindowPosition + new Vector2(0, GrapherWindowSize.Y), null, Color.Black, 0f, Vector2.Zero, new Vector2(GrapherWindowSize.X + 2, 2), SpriteEffects.None, 0.001f);

            btn_close.Draw();
            btn_median.Draw();
            btn_average.Draw();
            btn_highest.Draw();
            btn_lowest.Draw();

            List<float> vals = new List<float>();

            List<Generation> gens = new List<Generation>();

            foreach (Generation ge in GeneticAlgorithm.thisEvolution.Generations)
            {
                gens.Add(ge);
            }

            gens.Add(new Generation(GeneticAlgorithm.CurrentGeneration));

            for (int i = 0; i < gens.Count / XScale; i++)
            {
                float val = 0;

                for (int j = 0; j < XScale; j++)
                {
                    switch (graphType)
                    {
                        case Grapher.Average:
                            val += gens[i * XScale + j].AverageFitness;

                            break;

                        case Grapher.Median:
                            val += gens[i * XScale + j].MedianFitness;

                            break;

                        case Grapher.Top:
                            val += gens[i * XScale + j].TopFitness;

                            break;

                        case Grapher.Lowest:
                            val += gens[i * XScale + j].LowestFitness;

                            break;
                    }
                }

                val /= XScale;

                val = MathHelper.Clamp(val, 0, val);

                vals.Add(val);
            }

            if (vals.Count > 0)
                graph.MaxValue = GetHighest(vals.ToArray());

            graph.Draw(vals, Color.Red);

            GameHelper.SpriteBatch.End();
            GameHelper.SpriteBatch.Begin();

            if (MouseWithinWindow())
            {
                if (InputManager.MouseVelocityScreen == Vector2.Zero)
                    timeHovered += (float)GameHelper.GameTime.ElapsedGameTime.TotalSeconds;
                else
                    timeHovered = 0;

                int mouseGeneration = -1;

                float deltaX = (float)graph.Size.X / vals.Count;

                float mouseX = InputManager.MousePositionScreen.X;
                mouseX = mouseX - GrapherWindowPosition.X - 20;

                mouseGeneration = (int)Math.Round(mouseX / deltaX);

                if (mouseGeneration + 1 != vals.Count && mouseGeneration < vals.Count)
                {
                    GameHelper.DrawLine(new Vector2(deltaX * mouseGeneration + GrapherWindowPosition.X + 20, GrapherWindowPosition.Y + 50), new Vector2(deltaX * mouseGeneration + GrapherWindowPosition.X + 20, GrapherWindowPosition.Y + GrapherWindowSize.Y - 20), Color.Red * 0.5f, 1, 0f);
                    DrawToolTip(new Vector2(deltaX * mouseGeneration + GrapherWindowPosition.X + 25, GrapherWindowPosition.Y + 20 + graph.Size.Y - ((vals[mouseGeneration * XScale] / graph.MaxValue) * graph.Size.Y)), "Gen: " + mouseGeneration * XScale + "\n" + graphType.ToString() + ": " + vals[mouseGeneration * XScale].ToString());
                }

                if (timeHovered > timeToHover)
                {
                    // Show tooltip.

                    DrawToolTip(InputManager.MousePositionScreen + new Vector2(30, 0), "Right Click: Simulate\nLeft Shift + Right Click: View");
                }

                if (InputManager.PressedMouseRight() && gen == -1)
                {
                    if (!InputManager.KeyPressing(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    {
                        // Load generation.

                        ScreenEvo.btn_resumeEvolution.Clickable = true;

                        gen = GeneticAlgorithm.Generation;
                        saved = new Generation(GeneticAlgorithm.CurrentGeneration).CloneEvolvis();

                        List<Evolvi> loadGen = GeneticAlgorithm.thisEvolution.Generations[mouseGeneration * XScale].CloneEvolvis();

                        GeneticAlgorithm.Generation = mouseGeneration;
                        GeneticAlgorithm.CurrentGeneration = loadGen;

                        GeneticAlgorithm.GenerationTime = 0;
                        GeneticAlgorithm.ElapsedGenerationTime = 0;

                    }
                    else
                    {
                        // VIEW INDIVIDUALS OF THAT GENERATION

                        if(GeneticAlgorithm.EvolutionSpeed != 0)
                            ScreenEvo.btn_pauseEvo.SimulatePress();

                        MyIndividualViewer.Viewing = new Generation(GeneticAlgorithm.thisEvolution.Generations[mouseGeneration * XScale].CloneEvolvis());
                        MyIndividualViewer.IsActive = true;
                        
                    }
                }
            }
            else
            {

                timeHovered = 0;
            }
        }

        public static void Resume()
        {
            GeneticAlgorithm.Generation = gen;
            GeneticAlgorithm.CurrentGeneration = new Generation(saved).CloneEvolvis();

            GeneticAlgorithm.GenerationTime = 180;
            GeneticAlgorithm.ElapsedGenerationTime = 0;

            gen = -1;
            saved = new List<Evolvi>();
        }

        public static float GetHighest(float[] arr)
        {
            float highest = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] > highest)
                {
                    highest = arr[i];
                }
            }

            return highest;
        }

        public static void DrawToolTip(Vector2 position, string text)
        {
            Vector2 stringSize = GameHelper.Font.MeasureString(text) + new Vector2(20);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], position, null, new Color(80, 80, 80, 200), 0f, Vector2.Zero, stringSize, SpriteEffects.None, 0f);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, text, position + new Vector2(10), Color.White);
        }
    }
}
