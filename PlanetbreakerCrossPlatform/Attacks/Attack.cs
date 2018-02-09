using Microsoft.Xna.Framework.Graphics;

using Planetbreaker;
using Planetbreaker.Utilities;


namespace Planetbreaker.Attacks
{
    internal abstract class Attack : GameEntity
    {
        internal readonly int Power;
        internal readonly DamageType DamageType;

        protected Attack(int power, DamageType damageType, IHitbox area, Texture2D texture, int layer) 
            : base(area, texture, layer)
        {
            Power = power;
            DamageType = damageType;
        }

        internal bool CollidesWith(GameEntity other)
        {
            if (Layer == other.Layer && Area.DetectCollision(other.Area))
            {
                ShouldDie = true;
                return true;
            }
            return false;
        }
    }
}
