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
    public static class InputManager
    {
        private static List<Keys> currentPressedKeys;
        private static List<Keys> previousPressedKeys;

        private static MouseState currentMouseState;
        private static MouseState previousMouseState;

        public static Rectangle MouseBoxWorld { get; set; }
        public static Rectangle MouseBoxScreen { get; set; }

        public static Vector2 MousePositionWorld { get; set; }
        private static Vector2 PreviousMousePositionWorld { get; set; }
        public static Vector2 MouseVelocityWorld { get; set; }

        public static Vector2 MousePositionScreen { get; set; }
        private static Vector2 PreviousMousePositionScreen { get; set; }
        public static Vector2 MouseVelocityScreen { get; set; }

        static InputManager()
        {
            currentPressedKeys = new List<Keys>();
            previousPressedKeys = new List<Keys>();

            
        }

        public static void SetTextInputEvent(EventHandler<TextInputEventArgs> tie)
        {
            GameHelper.Window.TextInput += tie;
        }

        public static void Start()
        {
            currentPressedKeys = Keyboard.GetState().GetPressedKeys().ToList();
            currentMouseState = Mouse.GetState();

            MousePositionWorld = Vector2.Transform(MousePositionScreen, Camera.Instance.InvertedMatrix);
            MouseVelocityWorld = MousePositionWorld - PreviousMousePositionWorld;

            MousePositionScreen = currentMouseState.Position.ToVector2();
            MouseVelocityScreen = MousePositionScreen - PreviousMousePositionScreen;

            MouseBoxScreen = new Rectangle(MousePositionScreen.ToPoint(), new Point(1));
            MouseBoxWorld = new Rectangle(MousePositionWorld.ToPoint(), new Point(1));
        }
        public static void End()
        {
            previousPressedKeys = currentPressedKeys;
            previousMouseState = currentMouseState;

            PreviousMousePositionWorld = MousePositionWorld;
            PreviousMousePositionScreen = MousePositionScreen;
        }

        public static bool KeyPressed(Keys k)
        {
            return (currentPressedKeys.Contains(k) && !previousPressedKeys.Contains(k)) && GameHelper.Game.IsActive;
        }

        public static List<Keys> KeyPressed(params Keys[] keys)
        {
            List<Keys> pressed = null;

            foreach (Keys k in keys)
            {
                if (currentPressedKeys.Contains(k) && !previousPressedKeys.Contains(k) && GameHelper.Game.IsActive)
                {
                    if (pressed == null) pressed = new List<Keys>();
                    pressed.Add(k);
                }
            }

            return pressed;
        }

        public static bool KeyPressing(Keys k)
        {
            return (currentPressedKeys.Contains(k)) && GameHelper.Game.IsActive;
        }

        public static List<Keys> KeyPressing(params Keys[] keys)
        {
            List<Keys> pressing = null;

            foreach (Keys k in keys)
            {
                if (currentPressedKeys.Contains(k) && GameHelper.Game.IsActive)
                {
                    if (pressing == null) pressing = new List<Keys>();

                    pressing.Add(k);
                }
            }

            return pressing;
        }

        public static Keys[] GetAllPressedKeys()
        {
            List<Keys> pressedKeys = new List<Keys>();

            foreach(Keys key in currentPressedKeys)
            {
                if (!previousPressedKeys.Contains(key) && GameHelper.Game.IsActive)
                {
                    pressedKeys.Add(key);
                }
            }

            return pressedKeys.ToArray();
        }

        public static Keys[] GetAllPressingKeys()
        {
            return currentPressedKeys.ToArray();
        }

        public static bool PressingMouseLeft()
        {
            return (currentMouseState.LeftButton == ButtonState.Pressed) && GameHelper.Game.IsActive;
        }
        public static bool PressedMouseLeft()
        {
            return (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released) && GameHelper.Game.IsActive;
        }
        public static bool PressingMouseRight()
        {
            return (currentMouseState.RightButton == ButtonState.Pressed) && GameHelper.Game.IsActive;
        }
        public static bool PressedMouseRight()
        {
            return (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released) && GameHelper.Game.IsActive;
        }
        public static bool ReleasedMouseLeft()
        {
            return (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed) && GameHelper.Game.IsActive;
        }
        public static bool ReleasedMouseRight()
        {
            return (currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed) && GameHelper.Game.IsActive;
        }

        public static bool ScrolledUp()
        {
            return (currentMouseState.ScrollWheelValue > previousMouseState.ScrollWheelValue) && GameHelper.Game.IsActive;
        }

        public static bool ScrolledDown()
        {
            return (currentMouseState.ScrollWheelValue < previousMouseState.ScrollWheelValue) && GameHelper.Game.IsActive;
        }
    }
}
