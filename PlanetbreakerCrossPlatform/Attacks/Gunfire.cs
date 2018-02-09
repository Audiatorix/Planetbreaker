using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Planetbreaker.Utilities;


namespace Planetbreaker.Attacks
{
    class Gunfire : Attack
    {
        internal static Texture2D Texture;

        internal Gunfire(Point loc, float direction, int layer, int power = 2) 
            : base(power, DamageType.GUNFIRE, new RectHitbox(loc.X, loc.Y, Texture.Width, Texture.Height), Texture, layer)
        {
            SetVel(25, direction);
        }
    }
}
