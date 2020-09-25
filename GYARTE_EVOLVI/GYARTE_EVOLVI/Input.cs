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
    public class Input
    {
        public Vector2 start;
        public Vector2 end;
        public float value;

        public Input(Vector2 s, Vector2 e, float v)
        {
            start = s;
            end = e;
            value = v;
        }

        public Input()
        {

        }
    }
}
