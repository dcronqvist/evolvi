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
    public class Evolvi
    {
        private float _visionRange = 750f;
        private int _areasOfVision = 11;
        private int _fov = 90;

        private string TextureName { get; set; }
        public Texture2D TextureBody
        {
            get
            {
                return GameHelper.TextureManager[TextureName];
            }
        }
        
        internal Input[] Inputs { get; set; }
        public Network Network { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Movement { get; set; }
        public Vector2 LastPosition { get; set; }
        public Color Color { get; set; }
        public Rectangle CollisionBox { get; set; }

        private float _angle;
        public float Speed { get; set; }
        public float AngleRadians
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
            }
        }
        public float AngleDegrees
        {
            get
            {
                return MathHelper.ToDegrees(AngleRadians);
            }
            set
            {
                AngleRadians = MathHelper.ToDegrees(value);
            }
        }

        public int AreasOfVision { get; set; }
        public float VisionRange { get; set; }
        public int FOV { get; set; }

        public float Energy { get; set; }
        public float FoodEaten { get; set; }
        public float TimeAlive { get; set; }
        public bool Dead { get; set; }
        public float AllowedStandStillTime
        {
            get
            {
                return 10f;
            }
        }
        public float StandStillTime { get; set; }

        public Evolvi()
        {
            TextureName = "Evolvi_Body";
        }

        public Evolvi(Vector2 position)
        {
            Dead = false;
            Position = position;
            _angle = 0f;
            VisionRange = _visionRange;
            AreasOfVision = _areasOfVision;
            FOV = _fov;

            Inputs = new Input[AreasOfVision + 1];
            Energy = 1000;
            FoodEaten = 0;
            TextureName = "Evolvi_Body";

            Network = new Network(new int[] { AreasOfVision + 2, AreasOfVision - 2, 7, 5, 3 });

            Color = Color.Black;

            CollisionBox = new Rectangle(Position.ToPoint() - (TextureBody.Bounds.Size.ToVector2() / 2f).ToPoint(), TextureBody.Bounds.Size);
        }

        public Evolvi(Vector2 position, bool networkOverride)
        {
            Dead = false;
            Position = position;
            _angle = 0f;
            VisionRange = _visionRange;
            AreasOfVision = _areasOfVision;
            FOV = _fov;

            Inputs = new Input[AreasOfVision + 1];
            Energy = 1000;
            FoodEaten = 0;
            TextureName = "Evolvi_Body";

            if (networkOverride)
                Network = new Network(new int[] { AreasOfVision + 2, AreasOfVision - 2, 7, 5, 3 });

            Color = Color.Black;

            CollisionBox = new Rectangle(Position.ToPoint() - (TextureBody.Bounds.Size.ToVector2() / 2f).ToPoint(), TextureBody.Bounds.Size);
        }

        public void Mutate(float mutationRate)
        {
            Random RNG = new Random();

            for (int l = 1; l < Network.LayerCount; l++)
            {
                Layer layer = Network.Layers[l];

                for (int n = 0; n < layer.NeuronCount; n++)
                {
                    Neuron neuron = Network.Layers[l].Neurons[n];

                    for (int c = 0; c < neuron.ConnectionCount; c++)
                    {
                        Connection connection = Network.Layers[l].Neurons[n].Connections[c];

                        if (RNG.NextDouble() < mutationRate)
                        {
                            double coolieo = RNG.NextDouble();

                            if (coolieo < 0.05)
                            {
                                connection.Weight *= -1d;
                            }
                            else
                            {
                                connection.Weight += GameHelper.RNGNegPos(0.1);
                            }
                        }

                        //Thread.Sleep(10);
                    }

                    if (RNG.NextDouble() < mutationRate && l != Network.LayerCount - 1)
                    {
                        double coolio = RNG.NextDouble();

                        if (coolio < 0.05)
                        {
                            neuron.Bias *= -1d;
                        }
                        else
                        {
                            neuron.Bias += GameHelper.RNGNegPos(0.005);
                        }

                        coolio = RNG.NextDouble();

                        if(coolio < 0.05)
                        {
                            neuron.Bias = 0;
                        }
                    }
                }
            }
        }

        public void Update(Input[] inpts)
        {
            TimeAlive += ((float)GameHelper.GameTime.ElapsedGameTime.TotalSeconds);

            Inputs = inpts;

            List<double> passedInputs = new List<double>();

            for (int i = 0; i < Inputs.Length; i++)
            {
                passedInputs.Add(Inputs[i].value);
            }

            passedInputs.Add(Speed);

            double[] results = Network.Run(passedInputs);

            double LEFT = results[0];
            double MIDDLE = results[1];
            double RIGHT = results[2];

            double resultAngle = LEFT - RIGHT;
            Speed += (float)MIDDLE;

            Speed = MathHelper.Clamp(Speed, -2, 2);

            AngleRadians += (float)resultAngle;

            Movement = new Vector2((float)Math.Cos(AngleRadians), (float)Math.Sin(AngleRadians));

            Position += Movement * Speed;

            Energy -= ((float)Movement.Length() * Math.Abs(Speed));

            if(Math.Abs(Movement.Length() * Speed) < 0.2)
            {
                StandStillTime += (float)GameHelper.GameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                StandStillTime = 0;
            }

            LastPosition = Position;

            if(StandStillTime > AllowedStandStillTime)
            {
                Energy = 0;
            }

            CollisionBox = new Rectangle(Position.ToPoint() - (TextureBody.Bounds.Size.ToVector2() / 2f).ToPoint(), TextureBody.Bounds.Size);

            // POSITION FIXATION
            if((Position.X + Movement.X) <= 0)
            {
                Energy = 0;

            }
            if (Position.X + Movement.X >= 2000)
            {
                Energy = 0;

            }
            if (Position.Y + Movement.Y <= 0)
            {
                Energy = 0;

            }
            if (Position.Y + Movement.Y >= 2000)
            {
                Energy = 0;
            }
        }

        public void Draw()
        {
            GameHelper.SpriteBatch.Draw(TextureBody, Position, null, Color, AngleRadians, TextureBody.Bounds.Size.ToVector2() / 2f, 1f, SpriteEffects.None, 0.001f);
            GameHelper.DrawLine(Position, Position + (Movement * 10f), Color, 1, 0.01f);
            GameHelper.SpriteBatch.Draw(TextureBody, Position + (Movement * 10f), null, Color, AngleRadians, TextureBody.Bounds.Size.ToVector2() / 2f, 0.5f, SpriteEffects.None, 0.001f);

            if (GameHelper.DEBUG_MODE)
            {

                if (GameHelper.SHOW_ENERGY)
                {
                    GameHelper.SpriteBatch.DrawString(GameHelper.Font, Energy.ToString(), Position + new Vector2(10, 10), Color.Black);
                    GameHelper.SpriteBatch.DrawString(GameHelper.Font, (Movement.Length() * Speed).ToString(), Position + new Vector2(10, 40), Color.Black);
                    GameHelper.SpriteBatch.DrawString(GameHelper.Font, StandStillTime.ToString(), Position + new Vector2(10, 70), Color.Black);
                }

                if (GameHelper.SHOW_VF)
                {
                    foreach (Input i in Inputs)
                    {
                        if (i.value >= 0)
                            GameHelper.DrawLine(i.start, i.end, Color, 2, 0.01f);
                        else
                            GameHelper.DrawLine(i.start, i.end, Color.Red, 2, 0.01f);

                        GameHelper.SpriteBatch.Draw(TextureBody, i.end, null, Color, AngleRadians, TextureBody.Bounds.Size.ToVector2() / 2f, 1f, SpriteEffects.None, 0.001f);
                        GameHelper.SpriteBatch.DrawString(GameHelper.Font, i.value.ToString(), i.end, Color.Black);

                    }
                }
            }
        }

        public void CustomDraw(Vector2 pos, float angle, float scale)
        {
            GameHelper.SpriteBatch.End();
            GameHelper.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, null);

            GameHelper.SpriteBatch.Draw(TextureBody, pos, null, Color, angle, TextureBody.Bounds.Size.ToVector2() / 2f, scale, SpriteEffects.None, 0.001f);
            GameHelper.DrawLine(pos, pos + (Movement * 10f), Color, 1, 0.001f);
            GameHelper.SpriteBatch.Draw(TextureBody, pos + (Movement * 10f), null, Color, angle, TextureBody.Bounds.Size.ToVector2() / 2f, scale / 2f, SpriteEffects.None, 0.001f);

            GameHelper.SpriteBatch.End();
            GameHelper.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, null);
        }

        public void CustomDraw(Vector2 pos, float angle, float scale, Color c)
        {
            GameHelper.SpriteBatch.End();
            GameHelper.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, null);

            GameHelper.SpriteBatch.Draw(TextureBody, pos, null, c, angle, TextureBody.Bounds.Size.ToVector2() / 2f, scale, SpriteEffects.None, 0.001f);
            GameHelper.DrawLine(pos, pos + (Movement * 10f), c, 1, 0.001f);
            GameHelper.SpriteBatch.Draw(TextureBody, pos + (Movement * 10f), null, c, angle, TextureBody.Bounds.Size.ToVector2() / 2f, scale / 2f, SpriteEffects.None, 0.001f);

            GameHelper.SpriteBatch.End();
            GameHelper.SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, null);
        }

        public void DrawSelected(float angle)
        {
            GameHelper.SpriteBatch.Draw(GameHelper.TextureManager["Evolvi_Selected"], Position, null, Color.LightGoldenrodYellow, angle, GameHelper.TextureManager["Evolvi_Selected"].Bounds.Size.ToVector2() / 2f, 0.5f, SpriteEffects.None, 0.001f);

        }

        public Evolvi Crossbreed(Evolvi e)
        {
            Random RNG = new Random();

            Vector2 pos = Position;

            pos = new Vector2(RNG.Next(0, 2000), RNG.Next(0, 2000));

            Evolvi newE = new Evolvi(pos + new Vector2(RNG.Next(-30, 30), RNG.Next(-30, 30)), false);

            //newE.color = (new Color((float)RNG.NextDouble(), (float)RNG.NextDouble(), ((float)RNG.NextDouble() * 2f) - 1f));
            newE.Network = this.Network.Crossbreed(e.Network);

            newE._angle = (float)(RNG.NextDouble() * MathHelper.TwoPi);

            return newE;
        }

        public Evolvi Clone()
        {
            Random RNG = new Random();

            Evolvi clone = new Evolvi(new Vector2(RNG.Next(0, 2000), RNG.Next(0, 2000)), false);

            clone._angle = _angle;
            clone.AreasOfVision = AreasOfVision;
            clone.FOV = FOV;
            clone.VisionRange = VisionRange;
            clone.CollisionBox = new Rectangle(CollisionBox.Location, CollisionBox.Size);
            clone.Color = Color;
            //clone.FoodEaten = FoodEaten;
            clone.FOV = FOV;
            clone.Inputs = new Input[AreasOfVision + 1];
            clone.TextureName = "Evolvi_Body";

            for (int i = 0; i < AreasOfVision + 1; i++)
            {
                clone.Inputs[i] = new Input();
            }

            clone.Network = Network.Clone();

            return clone;
        }


        public Evolvi CloneAll()
        {
            Random RNG = new Random();

            Evolvi clone = new Evolvi(new Vector2(RNG.Next(0, 2000), RNG.Next(0, 2000)), false);

            clone._angle = _angle;
            clone.AreasOfVision = AreasOfVision;
            clone.FOV = FOV;
            clone.VisionRange = VisionRange;
            clone.CollisionBox = new Rectangle(CollisionBox.Location, CollisionBox.Size);
            clone.Color = Color;
            clone.FoodEaten = FoodEaten;
            clone.FOV = FOV;
            clone.Inputs = new Input[AreasOfVision + 1];
            clone.TextureName = "Evolvi_Body";
            clone.Energy = Energy;

            for (int i = 0; i < AreasOfVision + 1; i++)
            {
                clone.Inputs[i] = new Input();
            }

            clone.Network = Network.Clone();

            return clone;
        }
    }
}
