using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Planetbreaker.Utilities;


namespace Planetbreaker.Attacks
{
    class Missile : Attack
    {
        internal static Texture2D Texture;

        private GameEntity target;

        internal Missile(Point loc, float direction, GameEntity target, int layer, int power = 35)
            : base(power, DamageType.EXPLOSIVE, new RectHitbox(loc.X, loc.Y, Texture.Width, Texture.Height), Texture, layer)
        {
            this.target = target;
            SetVel(2, direction);
        }

        internal override void Update()
        {
            Rectangle r = target.Area.AsRectangle();
            Accelerate(1, r.Center.X, r.Center.Y);

            base.Update();
        }
    }
}
