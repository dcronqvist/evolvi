using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GYARTE_EVOLVI
{
    public static class GameHelper
    {
        public static TextureManager TextureManager { get; set; }
        public static GameTime GameTime { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static GraphicsDeviceManager Graphics { get; set; }
        public static GraphicsDevice GraphicsDevice { get; set; }
        public static SpriteFont Font { get; set; }
        public static GameWindow Window { get; set; }
        public static bool DEBUG_MODE { get; set; }
        public static Game Game { get; set; }

        public static bool SHOW_VF { get; set; }
        public static bool SHOW_ENERGY { get; set; }

        static GameHelper()
        {
            TextureManager = new TextureManager("Textures");
            DEBUG_MODE = false;
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color c, int width, float layerDepth)
        {
            Vector2 edge = end - start;
            // calculate angle to rotate line
            float angle =
                (float)Math.Atan2(edge.Y, edge.X);


            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"],
                new Rectangle(// rectangle defines shape of line and position of start of line
                    (int)start.X,
                    (int)start.Y,
                    (int)edge.Length(), //sb will strech the texture to fill this rectangle
                    width), //width of line, change this to make thicker line
                null,
                c, //colour of line
                angle,     //angle of line (calulated above)
                new Vector2(0), // point in line about which to rotate
                SpriteEffects.None,
                layerDepth);

        }

        public static void DrawBar(Rectangle rec, float maxValue, float currentValue, Color color, int outlineWidth)
        {
            if(outlineWidth < 1)
                SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Rectangle(rec.X, rec.Y, (int)(rec.Width * (currentValue / maxValue)), rec.Height), color);

            if (outlineWidth > 0)
            {
                SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], new Rectangle(rec.X, rec.Y, (int)(rec.Width * (currentValue / maxValue)), rec.Height), color);

                DrawLine(new Vector2(rec.X, rec.Y), new Vector2(rec.X + rec.Width, rec.Y), Color.Black, outlineWidth, 0);
                DrawLine(new Vector2(rec.X, rec.Y), new Vector2(rec.X, rec.Y + rec.Height), Color.Black, outlineWidth, 0);
                DrawLine(new Vector2(rec.X + rec.Width, rec.Y + rec.Height), new Vector2(rec.X, rec.Y + rec.Height), Color.Black, outlineWidth, 0);
                DrawLine(new Vector2(rec.X + rec.Width, rec.Y + rec.Height), new Vector2(rec.X + rec.Width, rec.Y), Color.Black, outlineWidth, 0);
            }
        }

        public static double RNGNegPos(double pos)
        {
            Random RNG = new Random();

            return ((RNG.NextDouble() * (pos * 2)) - pos);
        }
    }
}
