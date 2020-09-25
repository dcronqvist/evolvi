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
    class Camera
    {
        private static Camera instance;
        Vector2 position;
        Vector2 targetPosition;
        Matrix viewMatrix;
        Matrix invMatrix;
        public float Zoom
        {
            get
            {
                return targetZoom;
            }
            set
            {
                targetZoom = value;
            }
        }
        private float actualZoom { get; set; }
        public float Friction { get { return 0.1f; } }
        private float targetZoom { get; set; }

        public Vector2 Position { get { return position; } set { position = value; } }

        public Matrix ViewMatrix { get { return viewMatrix; } }
        public Matrix InvertedMatrix { get { return invMatrix; } }

        public Camera()
        {
            Zoom = 1f;
            actualZoom = 1f;
        }

        public static Camera Instance
        {
            get
            {
                if (instance != null) return instance;
                else instance = new Camera();

                return instance;
            }
        }

        public void Update()
        {
            actualZoom += (targetZoom - actualZoom) * 0.08f;

            position += new Vector2(((targetPosition.X - position.X) * Friction), (targetPosition.Y - position.Y) * Friction);

            viewMatrix = Matrix.CreateTranslation(new Vector3(-position, 0)) *
                    Matrix.CreateScale(actualZoom) *
                    Matrix.CreateTranslation(new Vector3(GameHelper.Graphics.PreferredBackBufferWidth / 2, GameHelper.Graphics.PreferredBackBufferHeight / 2, 0));

            invMatrix = Matrix.Invert(viewMatrix);
        }

        public void SetFocalPoint(Vector2 focalPosition, bool instant)
        {
            targetPosition = new Vector2(focalPosition.X, focalPosition.Y);

            if (instant)
                position = targetPosition;
        }

        public bool WithinView(Vector2 pos)
        {
            bool within = false;

            if (pos.X > position.X - GameHelper.Graphics.PreferredBackBufferWidth && pos.X < (position.X) + GameHelper.Graphics.PreferredBackBufferWidth)
            {
                if (pos.Y > position.Y - GameHelper.Graphics.PreferredBackBufferHeight && pos.Y < (position.Y) + GameHelper.Graphics.PreferredBackBufferHeight)
                {
                    within = true;
                }
            }

            return within;
        }
    }
}
