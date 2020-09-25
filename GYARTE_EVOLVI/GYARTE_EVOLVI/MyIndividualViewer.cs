using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYARTE_EVOLVI
{
    public static class MyIndividualViewer
    {
        public static Vector3 SelectedEvolvi { get; set; }

        public static Generation Viewing { get; set; }
        public static Vector2 ViewerPosition { get; set; }
        public static Vector2 ViewerSize { get; set; }
        public static bool IsActive { get; set; }

        static Button btn_close;

        static MyIndividualViewer()
        {
            SelectedEvolvi = new Vector3(-1, -1, -1);
            IsActive = false;

            ViewerPosition = new Vector2(100, 100);
            ViewerSize = new Vector2(600, 600);

            btn_close = new Button(ViewerPosition + new Vector2(10, 10), new Point(70, 30), "Close", Color.DarkGray);

            btn_close.Clicked += Btn_close_Clicked;
        }

        private static void Btn_close_Clicked(object sender, EventArgs e)
        {
            IsActive = false;
        }

        public static void Update()
        {
            btn_close.Update();
            btn_close.Position = ViewerPosition + new Vector2(10, 10);


            if (InputManager.PressingMouseLeft())
            {
                if (InputManager.MouseBoxScreen.Intersects(new Rectangle(ViewerPosition.ToPoint(), ViewerSize.ToPoint())))
                {
                    Vector2 offset = ViewerPosition - InputManager.MousePositionScreen;

                    ViewerPosition = InputManager.MousePositionScreen + offset;

                    ViewerPosition += InputManager.MouseVelocityScreen;
                }
            }

            btn_close.Position = ViewerPosition + new Vector2(10, 10);

        }

        public static void Draw()
        {
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ViewerPosition, null, new Color(80, 80, 80, 200), 0f, Vector2.Zero, ViewerSize, SpriteEffects.None, 0f);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ViewerPosition, null, Color.Black, 0f, Vector2.Zero, new Vector2(2, ViewerSize.Y), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ViewerPosition, null, Color.Black, 0f, Vector2.Zero, new Vector2(ViewerSize.X, 2), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ViewerPosition + new Vector2(ViewerSize.X, 0), null, Color.Black, 0f, Vector2.Zero, new Vector2(2, ViewerSize.Y), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ViewerPosition + new Vector2(0, ViewerSize.Y), null, Color.Black, 0f, Vector2.Zero, new Vector2(ViewerSize.X + 2, 2), SpriteEffects.None, 0.001f);

            btn_close.Draw();

            if (Viewing != null)
            {
                int amount = Viewing.Evolvis.Count();

                int squarRoot = (int)Math.Sqrt(Viewing.Evolvis.Count()) + 1;

                int x = (int)Math.Ceiling((double)amount / squarRoot);

                float xScale = ((ViewerSize.X) / x);
                float yScale = ((ViewerSize.Y - 80) / squarRoot);

                for (int yy = 0; yy < squarRoot; yy++)
                {
                    for (int xx = 0; xx < x; xx++)
                    {
                        int thisEvolvi = (yy * squarRoot) + xx;

                        if(thisEvolvi < amount)
                        {
                            if(Viewing.Evolvis[thisEvolvi] == Viewing.Fittest)
                                Viewing.Evolvis[thisEvolvi].CustomDraw(ViewerPosition + new Vector2(50, 80) + new Vector2(xScale * xx, yScale * yy), 0f, 2f, Color.Gold);
                            else
                                Viewing.Evolvis[thisEvolvi].CustomDraw(ViewerPosition + new Vector2(50, 80) + new Vector2(xScale * xx, yScale * yy), 0f, 2f);

                            Viewing.Evolvis[thisEvolvi].Movement = new Vector2(2.5f, 0);
                            GameHelper.SpriteBatch.DrawString(GameHelper.Font, (thisEvolvi + 1).ToString(), ViewerPosition + new Vector2(50, 80) + new Vector2(xScale * xx, yScale * yy) + new Vector2(0, yScale / 2f), Color.White);
                        }
                    }
                }

                int mouseGeneration = -1;

                Rectangle viewRec = new Rectangle(ViewerPosition.ToPoint() + new Point(0, 60), new Point((int)(squarRoot * yScale) + 100, (int)(x * xScale) + 160));

                if (InputManager.MouseBoxScreen.Intersects(viewRec))
                {
                    int mX = (int)Math.Round((InputManager.MousePositionScreen - ViewerPosition - new Vector2(50, 80)).X / xScale);
                    int mY = (int)Math.Round((InputManager.MousePositionScreen - ViewerPosition - new Vector2(50, 80)).Y / yScale);


                    mouseGeneration = ((mY * squarRoot) + mX);

                    if (mouseGeneration < amount)
                    {
                        if (InputManager.PressedMouseLeft())
                        {
                            SelectedEvolvi = new Vector3(mX, mY, mouseGeneration);
                        }
                    }

                    if(SelectedEvolvi.Z != -1)
                    {
                        GameHelper.DrawLine(ViewerPosition + new Vector2(50, 80) + new Vector2(xScale * SelectedEvolvi.X, yScale * SelectedEvolvi.Y), InputManager.MousePositionScreen + new Vector2(15, 0), Color.Black, 5, 0f);
                        //DrawToolTip(InputManager.MousePositionScreen + new Vector2(30, 0), mX.ToString() + "\n" + mY.ToString() + "\n" + mouseGeneration.ToString());
                        DrawToolTipIndividual(InputManager.MousePositionScreen + new Vector2(15, 0), Viewing.Evolvis[(int)SelectedEvolvi.Z]);
                    }
                }
            }
        }

        public static void DrawToolTip(Vector2 position, string text)
        {
            Vector2 stringSize = GameHelper.Font.MeasureString(text) + new Vector2(20);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], position, null, new Color(80, 80, 80, 200), 0f, Vector2.Zero, stringSize, SpriteEffects.None, 0f);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, text, position + new Vector2(10), Color.White);
        }

        public static void DrawToolTipIndividual(Vector2 position, Evolvi e)
        {
            Vector2 size = new Vector2(650, 650);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], position, null, new Color(80, 80, 80, 200), 0f, Vector2.Zero, size, SpriteEffects.None, 0f);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "Fitness: " + e.Energy.ToString(), position + new Vector2(10), Color.White);
            DrawNetwork(position + new Vector2(10), e.Network, (int)(size.X - 40) / e.Network.LayerCount, (int)(size.Y - 40) / e.Network.Layers[0].NeuronCount, size.Y / 2f);
        }

        public static void DrawNetwork(Vector2 pos, Network network, int xOffset, int yOffset, float baseoffset)
        {
            Network ne = network;
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
                        GameHelper.SpriteBatch.DrawString(GameHelper.Font, "B", pos + new Vector2(50 + i * xOffset - (float)((n.Bias * 64f) / 2f), baseoffset - yoffset + j * yOffset - (float)((n.Bias * 64f) / 2f)), Color.Black);
                    }

                    if (n.Bias > 0)
                    {
                        GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], pos + new Vector2(50 + i * xOffset - (float)((n.Bias * 64f) / 2f), baseoffset - yoffset + j * yOffset - (float)((n.Bias * 64f) / 2f)), null, Color.Pink, 0f, Vector2.Zero, (float)Math.Abs(n.Bias * 32f), SpriteEffects.None, 0f);
                    }
                    else
                    {
                        GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], pos + new Vector2(50 + i * xOffset - (float)((n.Bias * 64f) / 2f), baseoffset - yoffset + j * yOffset - (float)((n.Bias * 64f) / 2f)), null, Color.Green, 0f, Vector2.Zero, (float)Math.Abs(n.Bias * 32f), SpriteEffects.None, 0f);
                    }

                    GameHelper.SpriteBatch.DrawString(GameHelper.Font, val, pos + new Vector2(50 + i * xOffset, baseoffset - yoffset + j * yOffset), Color.Black);


                    for (int h = 0; h < n.ConnectionCount; h++)
                    {
                        Connection c = n.Connections[h];

                        Color co = Color.Black;

                        if (c.Weight > 0)
                        {
                            co = Color.White;
                        }

                        Vector2 start = pos + new Vector2(50 + i * xOffset, baseoffset - yoffset + j * yOffset);
                        Vector2 end = pos + new Vector2(50 - xOffset + i * xOffset, baseoffset - lastOffset + h * yOffset);

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
