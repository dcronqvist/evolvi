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
    public class Food
    {
        private Vector2 _position;
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                OnPropertyChanged(this, EventArgs.Empty);
            }
        }
        public Texture2D TextureFood { get; set; }

        public Rectangle CollisionBox { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public float Nutrition { get; set; }

        private float minNut = 500;
        private float maxNut = 1000;

        public event EventHandler PropertyChanged;

        public Food(Vector2 position)
        {
            Random RNG = new Random();

            Nutrition = RNG.Next((int)minNut, (int)maxNut);

            Position = position;
            TextureFood = GameHelper.TextureManager["Evolvi_Food"];

            CollisionBox = new Rectangle(Position.ToPoint(), new Point((int)(TextureFood.Bounds.Size.X * (Nutrition / maxNut)), (int)(TextureFood.Bounds.Size.Y * (Nutrition / maxNut))));
            BoundingBox = new BoundingBox(new Vector3(Position, 0), new Vector3(Position + CollisionBox.Size.ToVector2(), 0));

            PropertyChanged += Food_PropertyChanged;
        }

        private void Food_PropertyChanged(object sender, EventArgs e)
        {
            CollisionBox = new Rectangle(Position.ToPoint(), new Point((int)(TextureFood.Bounds.Size.X * (Nutrition / maxNut)), (int)(TextureFood.Bounds.Size.Y * (Nutrition / maxNut))));
            BoundingBox = new BoundingBox(new Vector3(Position, 0), new Vector3(Position + CollisionBox.Size.ToVector2(), 0));
        }

        protected virtual void OnPropertyChanged(object sender, EventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public void Draw()
        {
            GameHelper.SpriteBatch.Draw(TextureFood, CollisionBox, Color.Green);
        }
    }
}
