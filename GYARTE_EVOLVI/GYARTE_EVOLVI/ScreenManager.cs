using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYARTE_EVOLVI
{
    public static class ScreenManager
    {
        // Public properties
        public static Dictionary<string, Screen> Screens { get; set; }
        public static Screen CurrentScreen { get; set; }

        static ScreenManager()
        {
            Screens = new Dictionary<string, Screen>();
        }

        public static void AddScreen(string key, Screen screen)
        {
            Screens.Add(key, screen);

            if(Screens.Count == 1)
            {
                CurrentScreen = screen;
            }
        }

        public static void SetScreen(string key)
        {
            if (Screens.ContainsKey(key))
            {
                CurrentScreen = Screens[key];
                CurrentScreen.OnEntered(EventArgs.Empty);
                //Console.WriteLine("Set new screen to: " + key);
            }
        }

        public static Screen GetScreen(string key)
        {
            if (Screens.ContainsKey(key))
            {
                return Screens[key];
            }

            return null;
        }

        public static void Update()
        {
            CurrentScreen?.Update();
        }

        public static void Draw()
        {
            CurrentScreen?.Draw();
        }
    }
}
