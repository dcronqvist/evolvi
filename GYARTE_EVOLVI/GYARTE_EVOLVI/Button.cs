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
    public class Button
    {
        // Public properties
        public Vector2 Position
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
                OnPropertyChanged(EventArgs.Empty);
            }
        }
        public Point Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                OnPropertyChanged(EventArgs.Empty);
            }
        }
        public Rectangle Box
        {
            get
            {
                return _box;
            }
            set
            {
                _box = value;
                OnPropertyChanged(EventArgs.Empty);
            }
        }
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged(EventArgs.Empty);
            }
        }
        public bool Clickable { get => _clickable; set => _clickable = value; }

        // Internal members
        Vector2 _textPosition;
        string _text;
        Rectangle _box;
        Point _size;
        Vector2 _pos;
        bool _clickable;

        public Color Color { get; set; }

        public event EventHandler Clicked;
        public event EventHandler Released;
        event EventHandler PropertyChanged;

        public Button(Vector2 pos, Point size, string text)
        {
            _pos = pos;
            _size = size;
            _box = new Rectangle(pos.ToPoint(), size);
            Text = text;
            Color = Color.Gray;
            _clickable = true;
        }

        public Button(Vector2 pos, Point size, string text, Color color)
        {
            _pos = pos;
            _size = size;
            _box = new Rectangle(pos.ToPoint(), size);
            Text = text;
            Color = color;
            _clickable = true;
        }

        public Button(Vector2 pos, Point size, string text, Color color, bool clickable)
        {
            _pos = pos;
            _size = size;
            _box = new Rectangle(pos.ToPoint(), size);
            Text = text;
            Color = color;
            _clickable = clickable;
        }

        // Button Events

        public void SimulatePress()
        {
            OnClicked(EventArgs.Empty);
        }

        protected virtual void OnClicked(EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }

        protected virtual void OnReleased(EventArgs e)
        {
            Released?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(EventArgs e)
        {
            Vector2 middleButton = _pos + (new Vector2(_size.X / 2, _size.Y / 2));
            _textPosition = middleButton - GameHelper.Font.MeasureString(_text) / 2;

            _box = new Rectangle(Position.ToPoint(), Size);

            PropertyChanged?.Invoke(this, e);
        }

        public void Update()
        {
            if (InputManager.PressedMouseLeft())
            {
                if(InputManager.MouseBoxScreen.Intersects(Box))
                {
                    if(_clickable)
                        OnClicked(EventArgs.Empty);
                }
            }

            if (InputManager.ReleasedMouseLeft())
            {
                if (InputManager.MouseBoxScreen.Intersects(Box))
                {
                    if(_clickable)
                        OnReleased(EventArgs.Empty);
                }
            }
        }

        public void Draw()
        {
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], Box, Color);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, Text, _textPosition, Color.White);

            if (!_clickable)
            {
                GameHelper.DrawLine(Position, Position + Size.ToVector2(), Color.Red, 4, 0f);
                GameHelper.DrawLine(Position + new Vector2(0, Size.Y), Position + new Vector2(Size.X, 0), Color.Red, 4, 0f);
            }
        }

        public void Draw(float scale)
        {
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], Position, null, Color, 0f, Vector2.Zero, Box.Size.ToVector2() * scale, SpriteEffects.None, 0f);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, Text, _textPosition, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void Draw(Vector2 pos)
        {
            Position = pos;

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], Box, Color);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, Text, _textPosition, Color.White);

            if (!_clickable)
            {
                GameHelper.DrawLine(Position, Position + Size.ToVector2(), Color.Red, 4, 0f);
                GameHelper.DrawLine(Position + new Vector2(0, Size.Y), Position + new Vector2(Size.X, 0), Color.Red, 4, 0f);
            }
        }
    }
}
