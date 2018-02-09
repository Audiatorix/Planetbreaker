using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Planetbreaker.Utilities;


namespace Planetbreaker.Enemies
{
    abstract class Enemy : LivingGameEntity
    {
        protected Enemy(IHitbox area, Texture2D texture, Texture2D armoredTexture, Texture2D shieldedTexture,
            int hull, int armor, int shields, int layer)
            : base(area, texture, armoredTexture, shieldedTexture, hull, armor, shields, layer)
        {

        }
    }
}
