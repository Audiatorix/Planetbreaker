using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Planetbreaker.Utilities;


namespace Planetbreaker.Attacks
{
    internal class PhotonTorpedo : Attack
    {
        internal static Texture2D Texture;

        internal PhotonTorpedo(Point loc, float direction, int layer, int power = 20) 
            : base(power, DamageType.ENERGY, new RectHitbox(loc.X, loc.Y, Texture.Width, Texture.Height), Texture, layer)
        {
            SetVel(10, direction);
        }

        internal override void Update()
        {
            RotateBy(1f);

            base.Update();
        }
    }
}
