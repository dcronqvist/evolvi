using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Windows.Forms;

namespace GYARTE_EVOLVI
{
    public class ScreenEvo : Screen
    {
        bool SHOW_NETWORK = false;
        bool FOLLOW_SELECTED = false;
        bool AUTOSELECT_ON_NEWGEN = false;
        bool AUTOSELECT_BEST_PERFORMER = false;
        bool USING_CONSOLE = false;
        int pausedSpeed = 1;

        Button btn_exit;
        Button btn_debug;
        Button btn_showNetwork;
        Button btn_evoSpeed_1;
        Button btn_evoSpeed_5;
        Button btn_evoSpeed_10;
        public static Button btn_pauseEvo;
        Button btn_openConsole;
        Button btn_openGrapher;
        public static Button btn_resumeEvolution;

        Button btn_autoSelectOnNewGeneration;
        Button btn_autoSelectBestPerformer;

        float cameraSpeed = 12f;
        Vector2 camPos;

        Color backColor;

        Color ButtonClicked = new Color(160, 160, 160, 255);
        Color ButtonReleased = new Color(64, 64, 64, 255);

        Evolvi SELECTED_EVOLVI;
        float selectedAngle;

        Graph g;

        public ScreenEvo()
        {
            g = new Graph(GameHelper.GraphicsDevice, new Point(600, 500));
            g.Position = new Vector2(150, 150 + 500);
            g.MaxValue = 1;

            selectedAngle = 0;
            SELECTED_EVOLVI = null;

            pausedSpeed = 1;

            GameHelper.DEBUG_MODE = false;

            camPos = GameHelper.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2f;

            Name = "SC_EVO";

            backColor = new Color(200, 200, 200);

            base.Entered += this.ScreenEvo_Entered;

            btn_exit = new Button(new Vector2(10, 10), new Point(100, 50), "Exit", ButtonReleased);
            btn_debug = new Button(new Vector2(120, 10), new Point(120, 20), "Debug", ButtonReleased);
            btn_showNetwork = new Button(new Vector2(120, 40), new Point(120, 20), "Show Network", ButtonReleased);
            btn_evoSpeed_1 = new Button(new Vector2(360, 10), new Point(100, 50), "1x", ButtonReleased);
            btn_evoSpeed_5 = new Button(new Vector2(470, 10), new Point(100, 50), "5x", ButtonReleased);
            btn_evoSpeed_10 = new Button(new Vector2(580, 10), new Point(100, 50), "10x", ButtonReleased);
            btn_pauseEvo = new Button(new Vector2(250, 10), new Point(100, 50), "Paused", ButtonClicked);
            btn_autoSelectOnNewGeneration = new Button(new Vector2(690, 10), new Point(100, 20), "ASNG", ButtonReleased);
            btn_autoSelectBestPerformer = new Button(new Vector2(690, 40), new Point(100, 20), "ASBP", ButtonReleased);
            btn_openConsole = new Button(new Vector2(800, 10), new Point(100, 50), "Console", ButtonReleased);
            btn_openGrapher = new Button(new Vector2(910, 10), new Point(100, 50), "Grapher", ButtonReleased);
            btn_resumeEvolution = new Button(new Vector2(1020, 10), new Point(100, 50), "Return", ButtonReleased, false);

            btn_exit.Clicked += this.Btn_exit_Clicked;
            btn_debug.Clicked += this.Btn_debug_Clicked;
            btn_showNetwork.Clicked += Btn_showNetwork_Clicked;
            btn_evoSpeed_1.Clicked += this.Btn_evoSpeed_1_Clicked;
            btn_evoSpeed_5.Clicked += Btn_evoSpeed_5_Clicked;
            btn_evoSpeed_10.Clicked += Btn_evoSpeed_10_Clicked;
            btn_pauseEvo.Clicked += Btn_pauseEvo_Clicked;
            btn_autoSelectOnNewGeneration.Clicked += Btn_autoSelectOnNewGeneration_Clicked;
            btn_autoSelectBestPerformer.Clicked += Btn_autoSelectBestPerformer_Clicked;
            btn_openConsole.Clicked += Btn_openConsole_Clicked;
            btn_openConsole.Released += Btn_openConsole_Released;
            btn_openGrapher.Clicked += Btn_openGrapher_Clicked;
            btn_resumeEvolution.Clicked += Btn_resumeEvolution_Clicked;

            GeneticAlgorithm.NewGeneration += GeneticAlgorithm_NewGeneration;

            ScreenModal = new Modal();
            ScreenModal.Closed += ScreenModal_Closed;
        }

        private void Btn_resumeEvolution_Clicked(object sender, EventArgs e)
        {
            MyGrapher.Resume();
            btn_resumeEvolution.Clickable = false;
        }

        private void Btn_openGrapher_Clicked(object sender, EventArgs e)
        {
            MyGrapher.IsActive = true;
            USING_CONSOLE = false;
        }

        private void ScreenModal_Closed(ModalResult mr)
        {
            switch (mr)
            {
                case ModalResult.Yes:
                    SaveFileDialog sfd = new SaveFileDialog();

                    sfd.AddExtension = true;
                    sfd.DefaultExt = ".json";

                    sfd.ShowDialog();



                    GeneticAlgorithm.thisEvolution.Stop(sfd.FileName);
                    ScreenManager.SetScreen("SC_MENU");
                    break;

                case ModalResult.No:
                    ScreenManager.SetScreen("SC_MENU");
                    break;

                case ModalResult.Cancel:

                    break;
            }
        }

        private void GeneticAlgorithm_NewGeneration(object sender, EventArgs e)
        {
            MyConsole.SendMessage(new Message("New Gen: " + GeneticAlgorithm.Generation, Message.MessageType.Information));

            SELECTED_EVOLVI = null;
            FOLLOW_SELECTED = false;

            if (AUTOSELECT_ON_NEWGEN)
            {
                SELECTED_EVOLVI = GeneticAlgorithm.SortFittest().Last();
            }
        }

        #region BUTTONS

        private void Btn_openConsole_Clicked(object sender, EventArgs e)
        {
            // OPEN CONSOLE
            btn_openConsole.Color = ButtonClicked;

            USING_CONSOLE = true;           
        }

        private void Btn_openConsole_Released(object sender, EventArgs e)
        {
            // FIX BUTTON
            btn_openConsole.Color = ButtonReleased;
        }

        private void Btn_autoSelectBestPerformer_Clicked(object sender, EventArgs e)
        {
            AUTOSELECT_BEST_PERFORMER = !AUTOSELECT_BEST_PERFORMER;

            if (AUTOSELECT_BEST_PERFORMER)
                btn_autoSelectBestPerformer.Color = ButtonClicked;
            else
                btn_autoSelectBestPerformer.Color = ButtonReleased;
        }

        private void Btn_autoSelectOnNewGeneration_Clicked(object sender, EventArgs e)
        {
            AUTOSELECT_ON_NEWGEN = !AUTOSELECT_ON_NEWGEN;

            if (AUTOSELECT_ON_NEWGEN)
                btn_autoSelectOnNewGeneration.Color = ButtonClicked;
            else
                btn_autoSelectOnNewGeneration.Color = ButtonReleased;
        }

        private void Btn_evoSpeed_10_Clicked(object sender, EventArgs e)
        {
            ChangeEvolutionSpeed(10);
        }

        private void Btn_pauseEvo_Clicked(object sender, EventArgs e)
        {
            if (GeneticAlgorithm.EvolutionSpeed == 0)
            {
                // TO BE PLAYED
                ChangeEvolutionSpeed(pausedSpeed);
                btn_pauseEvo.Text = "Playing";
                btn_pauseEvo.Color = ButtonReleased;
            }
            else
            {
                // TO BE PAUSED
                pausedSpeed = GeneticAlgorithm.EvolutionSpeed;
                ChangeEvolutionSpeed(0);
                btn_pauseEvo.Text = "Paused";
                btn_pauseEvo.Color = ButtonClicked;
            }
        }

        private void Btn_showNetwork_Clicked(object sender, EventArgs e)
        {
            SHOW_NETWORK = !SHOW_NETWORK;

            if (SHOW_NETWORK)
                btn_showNetwork.Color = ButtonClicked;
            else
                btn_showNetwork.Color = ButtonReleased;
        }

        private void Btn_evoSpeed_5_Clicked(object sender, EventArgs e)
        {
            ChangeEvolutionSpeed(5);
        }

        private void Btn_evoSpeed_1_Clicked(object sender, EventArgs e)
        {
            ChangeEvolutionSpeed(1);
        }

        private void Btn_exit_Clicked(object sender, EventArgs e)
        {
            ScreenModal.ShowDialog("Would you like to save the best neural network?", ModalButtons.YesNoCancel);
        }

        private void Btn_debug_Clicked(object sender, EventArgs e)
        {
            GameHelper.DEBUG_MODE = !GameHelper.DEBUG_MODE;

            if (GameHelper.DEBUG_MODE)
                btn_debug.Color = ButtonClicked;
            else
                btn_debug.Color = ButtonReleased;
        }

        #endregion

        public void ChangeEvolutionSpeed(int newSpeed)
        {
            if (GeneticAlgorithm.EvolutionSpeed == 0)
            {
                // TO BE PLAYED
                btn_pauseEvo.Text = "Playing";
                btn_pauseEvo.Color = ButtonReleased;
            }
            else
            {
                // TO BE PAUSED
                //pausedSpeed = GeneticAlgorithm.EvolutionSpeed;
                //btn_pauseEvo.Text = "Paused";
                //btn_pauseEvo.Color = Color.Red;
            }

            btn_evoSpeed_1.Color = ButtonReleased;
            btn_evoSpeed_5.Color = ButtonReleased;
            btn_evoSpeed_10.Color = ButtonReleased;

            GeneticAlgorithm.EvolutionSpeed = newSpeed;

            switch (newSpeed)
            {
                case 1:
                    btn_evoSpeed_1.Color = ButtonClicked;
                    break;

                case 5:
                    btn_evoSpeed_5.Color = ButtonClicked;
                    break;

                case 10:
                    btn_evoSpeed_10.Color = ButtonClicked;
                    break;
            }
        }

        private void ScreenEvo_Entered(object sender, EventArgs e)
        {
            Camera.Instance.SetFocalPoint(camPos, true);
            GeneticAlgorithm.DoUpdate(GameHelper.GameTime);
        }

        public override void Update()
        {
            if (ScreenModal?.isActive == false)
            {
                GameHelper.Window.Title = "Time: " + Math.Round(GeneticAlgorithm.ElapsedGenerationTime) + " / " + GeneticAlgorithm.GenerationTime + " Generation: " + GeneticAlgorithm.Generation;

                btn_exit.Update();
                btn_debug.Update();
                btn_showNetwork.Update();
                btn_evoSpeed_1.Update();
                btn_evoSpeed_5.Update();
                btn_evoSpeed_10.Update();
                btn_pauseEvo.Update();
                btn_autoSelectOnNewGeneration.Update();
                btn_autoSelectBestPerformer.Update();
                btn_openConsole.Update();
                btn_openGrapher.Update();
                btn_resumeEvolution.Update();

                if (!USING_CONSOLE)
                {

                    #region Camera Handling
                    Camera.Instance.SetFocalPoint(camPos, false);
                    Camera.Instance.Update();

                    if (InputManager.ScrolledUp())
                    {
                        Camera.Instance.Zoom *= 1.1f;
                    }
                    if (InputManager.ScrolledDown())
                    {
                        Camera.Instance.Zoom /= 1.1f;
                    }

                    if (InputManager.KeyPressing(Microsoft.Xna.Framework.Input.Keys.W))
                    {
                        FOLLOW_SELECTED = false;
                        camPos += new Vector2(0, -cameraSpeed / Camera.Instance.Zoom);
                    }
                    if (InputManager.KeyPressing(Microsoft.Xna.Framework.Input.Keys.S))
                    {
                        FOLLOW_SELECTED = false;
                        camPos += new Vector2(0, cameraSpeed / Camera.Instance.Zoom);
                    }
                    if (InputManager.KeyPressing(Microsoft.Xna.Framework.Input.Keys.A))
                    {
                        FOLLOW_SELECTED = false;
                        camPos += new Vector2(-cameraSpeed / Camera.Instance.Zoom, 0);
                    }
                    if (InputManager.KeyPressing(Microsoft.Xna.Framework.Input.Keys.D))
                    {
                        FOLLOW_SELECTED = false;
                        camPos += new Vector2(cameraSpeed / Camera.Instance.Zoom, 0);
                    }
                    #endregion

                    if (InputManager.PressedMouseLeft())
                    {
                        foreach (Evolvi e in GeneticAlgorithm.CurrentGeneration)
                        {
                            if (InputManager.MouseBoxWorld.Intersects(e.CollisionBox))
                            {
                                SELECTED_EVOLVI = e;
                                break;
                            }
                        }
                    }

                    if (InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                    {
                        FOLLOW_SELECTED = true;
                    }
                }

                if (USING_CONSOLE && InputManager.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    USING_CONSOLE = false;
                }

                if (AUTOSELECT_BEST_PERFORMER && GeneticAlgorithm.CurrentGeneration.Count > 0)
                {
                    SELECTED_EVOLVI = GeneticAlgorithm.SortFittest().Last();
                }

                if (FOLLOW_SELECTED && SELECTED_EVOLVI != null)
                {
                    camPos = SELECTED_EVOLVI.Position;
                }

                if (USING_CONSOLE)
                {
                    MyConsole.Update();
                }

                if (MyGrapher.IsActive)
                {
                    MyGrapher.Update();
                }

                if (MyIndividualViewer.IsActive)
                {
                    MyIndividualViewer.Update();
                }

                GeneticAlgorithm.Update(GameHelper.GameTime);

                selectedAngle += 0.05f;


                base.Update();
            }
            else
            {
                ScreenModal.Update();
            }
        }

        public override void Draw()
        {
            GameHelper.GraphicsDevice.Clear(backColor);

            // WORLD
            GameHelper.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, Camera.Instance.ViewMatrix);

            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "TESTING", new Vector2(20, 20), Color.Black);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, Camera.Instance.Zoom.ToString(), new Vector2(20, 40), Color.Black);

            // Borders
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], Vector2.Zero, null, Color.DarkGray, 0f, Vector2.Zero, new Vector2(2000), SpriteEffects.None, 0.0001f);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, new Vector2(4, 2000), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], Vector2.Zero, null, Color.Black, 0f, Vector2.Zero, new Vector2(2000, 4), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Vector2(2000, 0), null, Color.Black, 0f, Vector2.Zero, new Vector2(4, 2000), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Vector2(0, 2000), null, Color.Black, 0f, Vector2.Zero, new Vector2(2000, 4), SpriteEffects.None, 0.001f);

            // DRAW SHIT HERE

            GeneticAlgorithm.Draw();

            if(SELECTED_EVOLVI != null)
            {
                SELECTED_EVOLVI.DrawSelected(selectedAngle);
            }

            GameHelper.SpriteBatch.End();

            // GUI
            GameHelper.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, null);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Rectangle(0, 0, GameHelper.GraphicsDevice.Viewport.Width, 70), Color.DimGray * 0.7f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Rectangle(0, 70, GameHelper.GraphicsDevice.Viewport.Width, 2), Color.Black);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Rectangle(0, GameHelper.GraphicsDevice.Viewport.Height - 70, GameHelper.GraphicsDevice.Viewport.Width, 70), Color.DimGray * 0.7f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Rectangle(0, GameHelper.GraphicsDevice.Viewport.Height - 72, GameHelper.GraphicsDevice.Viewport.Width, 2), Color.Black);


            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "Generation: " + GeneticAlgorithm.Generation, new Vector2(10, GameHelper.GraphicsDevice.Viewport.Height - 70 + 15), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            if(GeneticAlgorithm.GenerationTime != 0)
                GameHelper.SpriteBatch.DrawString(GameHelper.Font, "Generation Lifetime: " + Math.Round(GeneticAlgorithm.ElapsedGenerationTime)  + " / " + GeneticAlgorithm.GenerationTime, new Vector2(10, GameHelper.GraphicsDevice.Viewport.Height - 70 + 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            else
                GameHelper.SpriteBatch.DrawString(GameHelper.Font, "Generation Lifetime: " + Math.Round(GeneticAlgorithm.ElapsedGenerationTime) + " / INF", new Vector2(10, GameHelper.GraphicsDevice.Viewport.Height - 70 + 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "(" + GeneticAlgorithm.EvolutionSpeed + "x)", new Vector2(215, GameHelper.GraphicsDevice.Viewport.Height - 70 + 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            btn_exit.Draw();
            btn_debug.Draw();
            btn_showNetwork.Draw();
            btn_evoSpeed_1.Draw();
            btn_evoSpeed_5.Draw();
            btn_evoSpeed_10.Draw();
            btn_pauseEvo.Draw();
            btn_autoSelectOnNewGeneration.Draw();
            btn_autoSelectBestPerformer.Draw();
            btn_openConsole.Draw();
            btn_openGrapher.Draw();
            btn_resumeEvolution.Draw();

            if (SHOW_NETWORK && SELECTED_EVOLVI != null)
            {
                DrawNetwork(SELECTED_EVOLVI.Network);
            }

            if (USING_CONSOLE)
            {
                MyConsole.Draw();
            }

            if (MyGrapher.IsActive)
            {
                MyGrapher.Draw();
            }

            if (MyIndividualViewer.IsActive)
            {
                MyIndividualViewer.Draw();
            }

            if(ScreenModal?.isActive == true)
            {
                ScreenModal.Draw();
            }

            GameHelper.SpriteBatch.End();

            base.Draw();
        }

        public float GetHighest(float[] arr)
        {
            float highest = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if(arr[i] > highest)
                {
                    highest = arr[i];
                }
            }

            return highest;
        }

        public void DrawNetwork(Network network)
        {
            Network ne = network;

            int xOffset = 200;
            int yOffset = 30;
            float baseoffset = 300;
            float lastOffset = 0;

            for (int i = 0; i < ne.LayerCount; i++)
            {
                Layer l = ne.Layers[i];
                float yoffset = l.Neurons.Count / 2f * yOffset;

                for (int j = 0; j < l.NeuronCount; j++)
                {
                    Neuron n = l.Neurons[j];


                    string te = n.ConnectionCount.ToString();

                    if (te.Length > 3)
                    {
                        te = te.Substring(0, 3);
                    }

                    string val = n.Value.ToString();

                    if (n.Value.ToString().Length > 4)
                    {
                        val = n.Value.ToString().Substring(0, 4);
                    }

                    if (n.Bias != 0)
                    {
                        GameHelper.SpriteBatch.DrawString(GameHelper.Font, "B", new Vector2(50 + i * xOffset - (float)((n.Bias * 64f) / 2f), baseoffset - yoffset + j * yOffset - (float)((n.Bias * 64f) / 2f)), Color.Black);
                    }

                    if (n.Bias > 0)
                    {
                        GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Vector2(50 + i * xOffset - (float)((n.Bias * 64f) / 2f), baseoffset - yoffset + j * yOffset - (float)((n.Bias * 64f) / 2f)), null, Color.Pink, 0f, Vector2.Zero, (float)Math.Abs(n.Bias * 32f), SpriteEffects.None, 0f);
                    }
                    else
                    {
                        GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Vector2(50 + i * xOffset - (float)((n.Bias * 64f) / 2f), baseoffset - yoffset + j * yOffset - (float)((n.Bias * 64f) / 2f)), null, Color.Green, 0f, Vector2.Zero, (float)Math.Abs(n.Bias * 32f), SpriteEffects.None, 0f);
                    }

                    GameHelper.SpriteBatch.DrawString(GameHelper.Font, val, new Vector2(50 + i * xOffset, baseoffset - yoffset + j * yOffset), Color.Black);


                    for (int h = 0; h < n.ConnectionCount; h++)
                    {
                        Connection c = n.Connections[h];

                        Color co = Color.Black;

                        if (c.Weight > 0)
                        {
                            co = Color.White;
                        }

                        Vector2 start = new Vector2(50 + i * xOffset, baseoffset - yoffset + j * yOffset);
                        Vector2 end = new Vector2(50 - xOffset + i * xOffset, baseoffset - lastOffset + h * yOffset);

                        string we = "";

                        if (c.Weight.ToString().Length > 4)
                        {
                            we = c.Weight.ToString().Substring(0, 4);
                        }

                        //spriteBatch.DrawString(fnt, we, ((start + end) / 2f), Color.Black,  + MathHelper.Pi, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                        if (c.Weight != 0)
                        {
                            GameHelper.DrawLine(start, end, co, MathHelper.Clamp((int)(Math.Abs(c.Weight * 10f)), 1, 10), 1f);
                        }
                    }

                }

                lastOffset = yoffset;
            }
        }
    }
}
