using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GYARTE_EVOLVI
{
    public class ScreenLoad : Screen
    {
        public ScreenLoad()
        {
            Name = "SC_LOAD";
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            GameHelper.GraphicsDevice.Clear(Color.Gray);

            GameHelper.SpriteBatch.Begin();

            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "Loading assets... ", new Vector2(20, 20), Color.Black);

            GameHelper.SpriteBatch.End();

            base.Draw();
        }
    }
}
