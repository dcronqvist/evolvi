using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Threading;

namespace GYARTE_EVOLVI
{
    public class ScreenLoadEvo : Screen
    {
        Thread loadingThread;

        bool initialized = false;

        Button btn_cancel;

        public ScreenLoadEvo() : base()
        {
            loadingThread = new Thread(() => GeneticAlgorithm.Initialize());
            Name = "SC_LOAD_EVO";

            Vector2 center = new Vector2(GameHelper.GraphicsDevice.Viewport.Width / 2f, GameHelper.GraphicsDevice.Viewport.Height / 2f);

            btn_cancel = new Button(center + new Vector2(-50, 100), new Point(100, 50), "Cancel", Color.DarkGray);

            btn_cancel.Clicked += this.Btn_cancel_Clicked;

            base.Entered += ScreenLoadEvo_Entered;
            ScreenModal.Closed += this.ScreenModal_Closed;
        }

        private void ScreenModal_Closed(ModalResult mr)
        {
            switch (mr)
            {
                case ModalResult.Yes:

                    loadingThread.Abort();
                    loadingThread = new Thread(() => GeneticAlgorithm.Initialize());

                    ScreenManager.SetScreen("SC_MENU");
                    break;

                case ModalResult.No:

                    break;
            }
        }

        private void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            ScreenModal.ShowDialog("Are you sure you want to cancel?", ModalButtons.YesNo);
        }

        private void ScreenLoadEvo_Entered(object sender, EventArgs e)
        {
            GeneticAlgorithm.Loading = true;

            loadingThread = new Thread(() => GeneticAlgorithm.Initialize());
            loadingThread.Start();
        }

        public override void Update()
        {
            btn_cancel.Update();

            if (ScreenModal.isActive)
            {
                ScreenModal.Update();
            }
            else
            {

                if (!GeneticAlgorithm.Loading)
                {
                    ScreenManager.SetScreen("SC_EVO");
                }

            }

            base.Update();
        }

        public override void Draw()
        {
            GameHelper.GraphicsDevice.Clear(Color.Gray);

            GameHelper.SpriteBatch.Begin();

            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "Loading...", new Vector2(GameHelper.GraphicsDevice.Viewport.Width / 2f, GameHelper.GraphicsDevice.Viewport.Height / 2f) - (GameHelper.Font.MeasureString("Loading...") / 2f), Color.White);
            GameHelper.DrawBar(new Rectangle((new Vector2(GameHelper.GraphicsDevice.Viewport.Width / 2f, GameHelper.GraphicsDevice.Viewport.Height / 2f) + new Vector2(-150, 50)).ToPoint(), new Point(300, 20)), GeneticAlgorithm.EvolviMaxAmount, GeneticAlgorithm.EvolviLoadedAmount, Color.Black, 1);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, Math.Round(((GeneticAlgorithm.EvolviLoadedAmount / (float)GeneticAlgorithm.EvolviMaxAmount) * 100)).ToString() + "%", new Vector2(GameHelper.GraphicsDevice.Viewport.Width / 2f, GameHelper.GraphicsDevice.Viewport.Height / 2f) + new Vector2(0, 62) - (GameHelper.Font.MeasureString(Math.Round(((GeneticAlgorithm.EvolviLoadedAmount / (float)GeneticAlgorithm.EvolviMaxAmount) * 100)).ToString() + "%") / 2f), Color.White);

            btn_cancel.Draw();

            if (ScreenModal.isActive)
            {
                ScreenModal.Draw();
            }

            GameHelper.SpriteBatch.End();

            base.Draw();
        }
    }
}
