using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace GYARTE_EVOLVI
{
    public class ScreenMenu : Screen
    {
        Button btn_start;
        Button btn_exit;
        Button btn_start_withoutload;

        public ScreenMenu()
        {
            Name = "SC_MENU";

            Vector2 center = (GameHelper.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2f);

            btn_start = new Button(center + new Vector2(-100, 50), new Point(200, 50), "Load from File", Color.DarkGray);
            btn_exit = new Button(center + new Vector2(-100, 130), new Point(200, 50), "Exit", Color.DarkGray);
            btn_start_withoutload = new Button(center + new Vector2(-100, -30), new Point(200, 50), "Start New Evolution", Color.DarkGray);

            btn_exit.Clicked += this.Btn_exit_Clicked;
            btn_start.Clicked += this.Btn_start_Clicked;
            btn_start_withoutload.Clicked += Btn_start_withoutload_Clicked;

            ScreenModal.Closed += ScreenModal_Closed;
        }

        private void ScreenModal_Closed(ModalResult mr)
        {
            if (mr == ModalResult.Yes)
            {
                ScreenManager.SetScreen("SC_LOAD_EVO");
            }
        }

        private void Btn_start_withoutload_Clicked(object sender, EventArgs e)
        {
            ScreenModal.ShowDialog("This will create a new, empty, evolution. \nProceed?", ModalButtons.YesNo);

        }

        private void Btn_start_Clicked(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Neural Network Json | *.json";
            DialogResult dr = ofd.ShowDialog();

            if(dr == DialogResult.OK)
            {
                Network n = Network.LoadFromFile(ofd.FileName);

                GeneticAlgorithm.Initialize(false);

                for (int i = 0; i < GeneticAlgorithm.evolviAmount; i++)
                {
                    Random RNG = new Random();

                    Evolvi j = new Evolvi(new Vector2(RNG.Next(100, 1900), RNG.Next(100, 1900)), false);
                    j.Network = n.Clone();

                    GeneticAlgorithm.CurrentGeneration.Add(j);

                    Thread.Sleep(50);
                }

                ScreenManager.SetScreen("SC_EVO");
            }
            else
            {
                ScreenModal.ShowDialog("You did not choose a file, try again.", ModalButtons.OK);
            }
        }

        private void Btn_exit_Clicked(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        public override void Update()
        {
            if (ScreenModal.isActive)
            {
                ScreenModal.Update();
            }
            else
            {
                btn_start.Update();
                btn_exit.Update();
                btn_start_withoutload.Update();
            }

            base.Update();
        }

        public override void Draw()
        {
            GameHelper.GraphicsDevice.Clear(Color.Gray);

            GameHelper.SpriteBatch.Begin();

            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "Evolution of Neural Networks through genetic algorithms", (GameHelper.Window.ClientBounds.Size.ToVector2() / 2f) + new Vector2(0, -100) + (-GameHelper.Font.MeasureString("Evolution of Neural Networks through genetic algorithms") / 2f), Color.White);

            btn_start.Draw();
            btn_exit.Draw();
            btn_start_withoutload.Draw();

            if (ScreenModal.isActive)
            {
                ScreenModal.Draw();
            }

            GameHelper.SpriteBatch.End();

            base.Draw();
        }
    }
}
