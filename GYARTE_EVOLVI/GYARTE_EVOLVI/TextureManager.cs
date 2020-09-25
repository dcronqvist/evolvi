using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using System.IO;

namespace GYARTE_EVOLVI
{
    public class TextureManager
    {
        // Public Properties
        public Dictionary<string, Texture2D> Textures { get; set; }
        public float LoadingProgress { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsLoading { get; set; }

        // Events
        public event EventHandler Loaded;
        public event EventHandler StartLoad;

        // Private Members
        public string RootDirectory { get; set; }

        // Internal Members

        public Texture2D this[string s] { get { return Textures[s]; } }

        public TextureManager(string rootDirectory)
        {
            // Initialize dictionary and manager
            Textures = new Dictionary<string, Texture2D>();

            this.RootDirectory = rootDirectory;
            LoadingProgress = 0f;
            IsLoaded = false;
            IsLoading = false;
        }

        // EVENTS FOR MANAGER
        protected virtual void OnLoaded(EventArgs e)
        {
            //Console.WriteLine("TextureManager Loaded!");
            Loaded?.Invoke(this, e);
        }

        protected virtual void OnStartLoad(EventArgs e)
        {
            //Console.WriteLine("TextureManager now loading...");
            StartLoad?.Invoke(this, e);
        }

        // MAIN LOAD METHOD
        public void Load(ContentManager content)
        {
            OnStartLoad(EventArgs.Empty);

            LoadingProgress = 0f;
            IsLoaded = false;
            IsLoading = true;
            Textures.Clear();

            Thread loadThread = new Thread(() => LoadOnOtherThread(content));
            loadThread.Start();
        }

        private void LoadOnOtherThread(ContentManager content)
        {
            // Get all texture files
            string[] allFiles = Directory.GetFiles(@"..\Debug\Content\" + RootDirectory + @"\", "*", SearchOption.AllDirectories);
            float loadProgressPerFile = (1f / allFiles.Length);

            foreach (string file in allFiles)
            {
                // Getting the filename
                string[] filePathSplit = file.Split(char.Parse(@"\"));
                string fileName = filePathSplit[filePathSplit.Length - 1];
                fileName = fileName.Split(char.Parse("."))[0];

                string fileToLoad = file.Substring(filePathSplit[0].Length + 1 + filePathSplit[1].Length + 1 + filePathSplit[2].Length + filePathSplit[3].Length + 1);
                fileToLoad = fileToLoad.Substring(0, fileToLoad.Length - 4);

                Thread.Sleep(50);

                // Finally adding the texture
                Textures.Add(fileName, content.Load<Texture2D>(RootDirectory + fileToLoad));

                LoadingProgress += loadProgressPerFile;
                //Console.WriteLine("TextureManager loading... " + Math.Round(LoadingProgress * 100).ToString() + "%");
            }

            LoadingProgress = 1f;
            IsLoaded = true;
            IsLoading = false;

            OnLoaded(EventArgs.Empty);
        }
    }
}
