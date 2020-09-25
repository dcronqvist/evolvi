using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace GYARTE_EVOLVI
{
    public class Command
    {
        public string Key { get; set; }
        public delegate void CommandExecution(string[] args);
        public event CommandExecution Execute;

        protected virtual void OnExecute(params string[] args)
        {
            Execute?.Invoke(args);
        }

        public void SimulateExecution(string[] args)
        {
            OnExecute(args);
        }

        public Command(string key, CommandExecution ce)
        {
            Key = key;
            Execute += ce;
        }
    }

    public class Message
    {
        public enum MessageType
        {
            Information,
            Command,
            Error,
            Warning
        }

        public string Text { get; set; }
        public MessageType Type { get; set; }
        public DateTime TimeSent { get; set; }

        public Message(string text)
        {
            Text = text;
            Type = MessageType.Information;
            TimeSent = DateTime.Now;
        }

        public Message(string text, MessageType mt)
        {
            Text = text;
            Type = mt;
            TimeSent = DateTime.Now;
        }
    }

    public static class MyConsole
    {
        public static List<Command> ConsoleCommands { get; set; }
        public static List<Message> ConsoleMessages { get; set; }
        public static string CurrentLine { get; set; }

        public static Vector2 ConsoleWindowPosition { get; set; }
        public static Vector2 ConsoleWindowSize { get; set; }

        public static int ScrollPosition { get; set; }

        static MyConsole()
        {
            ConsoleMessages = new List<Message>();
            ScrollPosition = ConsoleMessages.Count;

            ConsoleWindowPosition = new Vector2(100, 100);
            ConsoleWindowSize = new Vector2(460, 350);

            CurrentLine = "";

            ConsoleCommands = new List<Command>();

            ConsoleCommands.Add(new Command("exit", Exit_Execute));
            ConsoleCommands.Add(new Command("killProcess", KillProcess_Execute));
            ConsoleCommands.Add(new Command("evoSpeed", ChangeEvolutionSpeed_Execute));
            ConsoleCommands.Add(new Command("help", Commands_Execute));
            ConsoleCommands.Add(new Command("evoTime", EvoTime_Execute));
            ConsoleCommands.Add(new Command("pause", Pause_Execute));
            ConsoleCommands.Add(new Command("play", Play_Execute));
            ConsoleCommands.Add(new Command("infiniteTime", InfiniteTime_Execute));
            ConsoleCommands.Add(new Command("showVF", ShowVF_Execute));
            ConsoleCommands.Add(new Command("showEnergy", ShowEnergy_Execute));
            ConsoleCommands.Add(new Command("graphXScale", GraphXScale_Execute));

            SendMessage(new Message("/help for help with commands.", Message.MessageType.Warning));

            InputManager.SetTextInputEvent(TextInput);
        }

        private static void GraphXScale_Execute(string[] args)
        {
            int scale = int.Parse(args[0]);

            MyGrapher.XScale = scale;
        }

        private static void ShowEnergy_Execute(string[] args)
        {
            GameHelper.SHOW_ENERGY = !GameHelper.SHOW_ENERGY;
        }

        private static void ShowVF_Execute(string[] args)
        {
            GameHelper.SHOW_VF = !GameHelper.SHOW_VF;
        }

        static void TextInput(object sender, TextInputEventArgs e)
        {
            char backChar = (char)Keys.Back;
            char escapeChar = (char)Keys.Escape;
            char enterChar = (char)Keys.Enter;

            if (e.Character != enterChar)
            {
                if (e.Character != escapeChar)
                {
                    if (e.Character != backChar)
                    {
                        CurrentLine += e.Character;
                    }
                    else
                    {
                        if (CurrentLine != "")
                        {
                            CurrentLine = CurrentLine.Substring(0, CurrentLine.Length - 1);
                        }
                    }
                }
            }
        }

        #region Command Executors
        private static void InfiniteTime_Execute(params string[] args)
        {
            if(GeneticAlgorithm.GenerationTime == 0)
            {
                GeneticAlgorithm.GenerationTime = 60;
            }
            else
            {
                GeneticAlgorithm.GenerationTime = 0;
            }
        }

        private static void Play_Execute(params string[] args)
        {
            if (GeneticAlgorithm.EvolutionSpeed == 0)
            {
                ScreenEvo.btn_pauseEvo.SimulatePress();
                SendMessage(new Message("Evolution Playing.", Message.MessageType.Warning));
            }
            else
            {
                SendMessage(new Message("Evolution is already playing.", Message.MessageType.Error));
            }
        }

        private static void Pause_Execute(params string[] args)
        {
            if (GeneticAlgorithm.EvolutionSpeed != 0)
            {
                ScreenEvo.btn_pauseEvo.SimulatePress();
                SendMessage(new Message("Evolution Paused.", Message.MessageType.Warning));
            }
            else
            {
                SendMessage(new Message("Evolution is already paused.", Message.MessageType.Error));
            }
        }

        private static void Exit_Execute(params string[] args)
        {
            Process.GetCurrentProcess().Kill();
            //SendMessage(new Message("Exiting to Main Menu..."));
            //ScreenManager.SetScreen("SC_MENU");
        }

        private static void KillProcess_Execute(params string[] args)
        {
            Process.GetCurrentProcess().Kill();
        }

        private static void ChangeEvolutionSpeed_Execute(params string[] args)
        {
            
            if (args.Length > 0)
            {
                ((ScreenEvo)ScreenManager.GetScreen("SC_EVO")).ChangeEvolutionSpeed(int.Parse(args[0]));
                SendMessage(new Message("Changed EVO_SPEED to: " + args[0], Message.MessageType.Warning));
            }
            else
            {
                SendMessage(new Message("Try again. /evoSpeed <speed>", Message.MessageType.Error));
            }
        }

        private static void Commands_Execute(params string[] args)
        {
            foreach(Command c in ConsoleCommands.OrderBy(x => x.Key).ToList())
            {
                SendMessage(new Message(c.Key, Message.MessageType.Information));
            }
        }

        private static void EvoTime_Execute(params string[] args)
        {
            GeneticAlgorithm.GenerationTime = int.Parse(args[0]);
            SendMessage(new Message("Set GenerationTime to: " + GeneticAlgorithm.GenerationTime, Message.MessageType.Information));
        }

        #endregion

        public static void Update()
        {
            if (InputManager.ScrolledDown())
            {
                ScrollPosition -= 1;
            }
            if (InputManager.ScrolledUp())
            {
                ScrollPosition += 1;
            }

            ScrollPosition = MathHelper.Clamp(ScrollPosition, 0, ConsoleMessages.Count - 9);

            if (InputManager.PressingMouseLeft())
            {
                if(InputManager.MouseBoxScreen.Intersects(new Rectangle(ConsoleWindowPosition.ToPoint(), ConsoleWindowSize.ToPoint())))
                {
                    Vector2 offset = ConsoleWindowPosition - InputManager.MousePositionScreen;

                    ConsoleWindowPosition = InputManager.MousePositionScreen + offset;

                    ConsoleWindowPosition += InputManager.MouseVelocityScreen;
                }
            }

            Keys[] pressedKeys = InputManager.GetAllPressedKeys();

            if (pressedKeys.Contains(Keys.Enter))
            {
                if (CurrentLine == "")
                {
                    return;
                }
                else
                {
                    if (CurrentLine[0] == char.Parse("/"))
                    {
                        // BEGIN PARSING

                        string[] splitIntoWords = CurrentLine.Split(char.Parse(" "));

                        string key = splitIntoWords[0].Remove(0, 1);
                        //key = key.Remove(key.Length - 1, 1);

                        string[] args = new string[splitIntoWords.Length - 1];

                        for (int i = 1; i < splitIntoWords.Length; i++)
                        {
                            args[i - 1] = splitIntoWords[i];
                        }

                        foreach (Command command in ConsoleCommands)
                        {
                            if (command.Key == key)
                            {
                                command.SimulateExecution(args);
                                CurrentLine = "";
                                
                                return;
                            }
                        }

                        SendMessage(new Message("Invalid Command: " + key, Message.MessageType.Error));
                    }
                    else
                    {
                        SendMessage(new Message("Must write a command.", Message.MessageType.Warning));
                    }
                }

                CurrentLine = "";
            }
        }

        public static void SendMessage(Message message)
        {
            ConsoleMessages.Insert(0, message);
            ScrollPosition = 0;
        }
        
        public static void Draw()
        {
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ConsoleWindowPosition, null, new Color(80, 80, 80, 200), 0f, Vector2.Zero, ConsoleWindowSize, SpriteEffects.None, 0f);

            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ConsoleWindowPosition, null, Color.Black, 0f, Vector2.Zero, new Vector2(2, ConsoleWindowSize.Y), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ConsoleWindowPosition, null, Color.Black, 0f, Vector2.Zero, new Vector2(ConsoleWindowSize.X, 2), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ConsoleWindowPosition + new Vector2(ConsoleWindowSize.X, 0), null, Color.Black, 0f, Vector2.Zero, new Vector2(2, ConsoleWindowSize.Y), SpriteEffects.None, 0.001f);
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ConsoleWindowPosition + new Vector2(0, ConsoleWindowSize.Y), null, Color.Black, 0f, Vector2.Zero, new Vector2(ConsoleWindowSize.X + 2, 2), SpriteEffects.None, 0.001f);

            if (ConsoleMessages.Count > 12)
            {
                GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["PIXEL"], ConsoleWindowPosition + new Vector2(ConsoleWindowSize.X - 4, ConsoleWindowSize.Y - (float)(((float)(ScrollPosition + 9) / (float)(ConsoleMessages.Count + 1)) * ConsoleWindowSize.Y)), null, Color.White, 0f, Vector2.Zero, new Vector2(2, 10), SpriteEffects.None, 0f);
            }
            // DRAW ALL MESSAGES
            for (int i = 0; i < ConsoleMessages.Count; i++)
            {
                if (i < 12)
                {
                    Vector2 pos = ConsoleWindowPosition + new Vector2(5, ConsoleWindowSize.Y - 20) + new Vector2(0, (-i * 25) - 25);

                    Color c = Color.White;

                    switch (ConsoleMessages[i + ScrollPosition].Type)
                    {
                        case Message.MessageType.Error:
                            c = Color.Red;
                            break;

                        case Message.MessageType.Warning:
                            c = Color.Yellow;
                            break;
                    }

                    GameHelper.SpriteBatch.DrawString(GameHelper.Font, ConsoleMessages[i + ScrollPosition].TimeSent.ToLongTimeString() + ": " + ConsoleMessages[i + ScrollPosition].Text, pos + new Vector2(1.5f), Color.Black);
                    GameHelper.SpriteBatch.DrawString(GameHelper.Font, ConsoleMessages[i + ScrollPosition].TimeSent.ToLongTimeString() + ": " + ConsoleMessages[i + ScrollPosition].Text, pos, c);
                }
            }

            GameHelper.SpriteBatch.DrawString(GameHelper.Font, CurrentLine, ConsoleWindowPosition + new Vector2(1.5f) + new Vector2(5, ConsoleWindowSize.Y - 20), Color.Black);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, CurrentLine, ConsoleWindowPosition + new Vector2(5, ConsoleWindowSize.Y - 20), Color.White);

            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "_", ConsoleWindowPosition + new Vector2(1.5f) + new Vector2(5, ConsoleWindowSize.Y - 20) + new Vector2(GameHelper.Font.MeasureString(CurrentLine).X, 0), Color.Black);
            GameHelper.SpriteBatch.DrawString(GameHelper.Font, "_", ConsoleWindowPosition + new Vector2(5, ConsoleWindowSize.Y - 20) + new Vector2(GameHelper.Font.MeasureString(CurrentLine).X, 0), Color.White);
        }

        public static string ConvertKeyToString(Keys key, bool usingShift)
        {
            string output = "";
            bool usesShift = usingShift;

            if (key >= Keys.A && key <= Keys.Z)
                output += key.ToString();
            else if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                output += ((int)(key - Keys.NumPad0)).ToString();
            else if (key >= Keys.D0 && key <= Keys.D9)
            {
                string num = ((int)(key - Keys.D0)).ToString();
                #region special num chars
                if (usesShift)
                {
                    switch (num)
                    {
                        case "1":
                            {
                                num = "!";
                            }
                            break;
                        case "2":
                            {
                                num = "\"";
                            }
                            break;
                        case "3":
                            {
                                num = "#";
                            }
                            break;
                        case "4":
                            {
                                num = "?";
                            }
                            break;
                        case "5":
                            {
                                num = "%";
                            }
                            break;
                        case "6":
                            {
                                num = "&";
                            }
                            break;
                        case "7":
                            {
                                num = "/";
                            }
                            break;
                        case "8":
                            {
                                num = "(";
                            }
                            break;
                        case "9":
                            {
                                num = ")";
                            }
                            break;
                        case "0":
                            {
                                num = "=";
                            }
                            break;
                        default:
                            //wtf?
                            break;
                    }
                }
                #endregion
                output += num;
            }
            else if (key == Keys.OemPeriod)
                output += ".";
            else if (key == Keys.OemTilde)
                output += "'";
            else if (key == Keys.Space)
                output += " ";
            else if (key == Keys.OemMinus)
                output += "-";
            else if (key == Keys.OemPlus)
                output += "+";
            else if (key == Keys.OemQuestion && usesShift)
                output += "?";
            else if (key == Keys.Back)
            {
                //backspace
                if(CurrentLine.Length >= 1)
                    CurrentLine = CurrentLine.Remove(CurrentLine.Length - 1);
            }

            if (!usesShift) //shouldn't need to upper because it's automagically in upper case
                output = output.ToLower();

            return output;
        }
    }
}
