using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Planetbreaker.Utilities;


namespace Planetbreaker.Enemies
{
    internal class BasicFighter : Enemy
    {
        private static Texture2D[] textures;
        internal static void SetTextures(Texture2D t, Texture2D at, Texture2D st)
        {
            textures = new[] { t, at, st };
        }

        internal BasicFighter(Point spawn, Point target) : base(
            new RectHitbox(spawn.X - 20, spawn.Y, textures[0].Width, textures[0].Height),
            textures[0], textures[1], textures[2], 1, 1, 1, 4)
        {
            //RotateTo((float) Math.Atan((target.X - spawn.X) / (spawn.Y - target.Y)));
            RotateTo((float) Math.PI);
            double arctan = Math.Atan((spawn.Y - target.Y) / (target.X - spawn.X));
            Accelerate(arctan < 0 ? .1 : -.1, arctan);
            //SetVel(2, 3 * Math.PI / 2);
        }
    }
}
