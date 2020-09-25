using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYARTE_EVOLVI
{
    public enum ModalResult
    {
        Yes,
        No,
        OK,
        Accept,
        Decline,
        Cancel,
    }

    public enum ModalButtons
    {
        YesNo,
        OK,
        AcceptDecline,
        YesNoCancel,
    }

    public class Modal
    {
        public bool isActive { get; set; }

        private Button btn_yes;
        private Button btn_no;
        private Button btn_ok;
        private Button btn_accept;
        private Button btn_decline;
        private Button btn_cancel;

        public Vector2 CenterScreen { get; set; }

        public string Text { get; set; }
        public ModalButtons Buttons { get; set; }

        public delegate void ModalClosed(ModalResult mr);

        public event ModalClosed Closed;

        public Modal()
        {
            isActive = false;

            InitializeButtons();
        }

        public void ShowDialog(string text)
        {
            isActive = true;
            Text = text;
            Buttons = ModalButtons.OK;
        }

        public void ShowDialog(string text, ModalButtons mb)
        {
            isActive = true;
            Text = text;
            Buttons = mb;
        }

        private void InitializeButtons()
        {
            CenterScreen = (GameHelper.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2f);
            Vector2 center = CenterScreen;


            btn_yes = new Button(center + new Vector2(-145, 50), new Point(90, 50), "Yes", Color.DarkGreen);
            btn_no = new Button(center + new Vector2(-45, 50), new Point(90, 50), "No", Color.DarkGreen);
            btn_cancel = new Button(center + new Vector2(55, 50), new Point(90, 50), "Cancel", Color.DarkGreen);

            btn_ok = new Button(center + new Vector2(10, 50), new Point(90, 50), "OK", Color.DarkGreen);

            btn_accept = new Button(center + new Vector2(-100, 50), new Point(90, 50), "Accept", Color.DarkGreen);
            btn_decline = new Button(center + new Vector2(10, 50), new Point(90, 50), "Decline", Color.DarkGreen);

            btn_yes.Clicked += Btn_yes_Clicked;
            btn_no.Clicked += Btn_no_Clicked;
            btn_cancel.Clicked += Btn_cancel_Clicked;
            btn_ok.Clicked += Btn_ok_Clicked;
            btn_accept.Clicked += Btn_accept_Clicked;
            btn_decline.Clicked += Btn_decline_Clicked;
        }

        private void Btn_cancel_Clicked(object sender, EventArgs e)
        {
            Closed?.Invoke(ModalResult.Cancel);
            isActive = false;
        }

        private void Btn_decline_Clicked(object sender, EventArgs e)
        {
            Closed?.Invoke(ModalResult.Decline);
            isActive = false;
        }

        private void Btn_accept_Clicked(object sender, EventArgs e)
        {
            Closed?.Invoke(ModalResult.Accept);
            isActive = false;

        }

        private void Btn_ok_Clicked(object sender, EventArgs e)
        {
            Closed?.Invoke(ModalResult.OK);
            isActive = false;

        }

        private void Btn_no_Clicked(object sender, EventArgs e)
        {
            Closed?.Invoke(ModalResult.No);
            isActive = false;

        }

        private void Btn_yes_Clicked(object sender, EventArgs e)
        {
            Closed?.Invoke(ModalResult.Yes);
            isActive = false;

        }

        public void Update()
        {
            switch (Buttons)
            {
                case ModalButtons.OK:
                    btn_ok.Update();
                    break;

                case ModalButtons.AcceptDecline:
                    btn_accept.Update();
                    btn_decline.Update();
                    break;

                case ModalButtons.YesNo:
                    btn_yes.Update();
                    btn_no.Update();
                    break;

                case ModalButtons.YesNoCancel:
                    btn_yes.Update();
                    btn_no.Update();
                    btn_cancel.Update();
                    break;
            }
        }

        public void Draw()
        {
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], Vector2.Zero, null, null, null, 0f, GameHelper.Window.ClientBounds.Size.ToVector2(), Color.Black * 0.75f, SpriteEffects.None, 0f);

            GameHelper.SpriteBatch.DrawString(GameHelper.Font, Text, (GameHelper.Window.ClientBounds.Size.ToVector2() / 2f) + (-GameHelper.Font.MeasureString(Text) / 2f), Color.White);

            switch (Buttons)
            {
                case ModalButtons.OK:
                    btn_ok.Draw();
                    break;

                case ModalButtons.AcceptDecline:
                    btn_accept.Draw();
                    btn_decline.Draw();
                    break;

                case ModalButtons.YesNo:
                    btn_yes.Draw(CenterScreen + new Vector2(-95, 50));
                    btn_no.Draw(CenterScreen + new Vector2(5, 50));
                    break;

                case ModalButtons.YesNoCancel:
                    btn_yes.Draw();
                    btn_no.Draw();
                    btn_cancel.Draw();
                    break;
            }


        }
    }
}
