using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Planetbreaker.Utilities;


namespace Planetbreaker
{
    internal abstract class LivingGameEntity : GameEntity
    {
        public int Hull    { get; protected set; }
        public int Armor   { get; protected set; }
        public int Shields { get; protected set; }
        private Texture2D armoredTexture, shieldedTexture;

        protected LivingGameEntity(IHitbox area, Texture2D texture, Texture2D armoredTexture, Texture2D shieldedTexture,
            int hull, int armor, int shields, int layer) : base(area, texture, layer)
        {
            ShouldDie = false;
            this.armoredTexture = armoredTexture;
            this.shieldedTexture = shieldedTexture;
            Hull = hull;
            Armor = armor;
            Shields = shields;
        }

        // True if entity should continue living, false otherwise
        internal void Damage(DamageType type, int power)
        {
            switch (type)
            {
                case DamageType.GUNFIRE:
                    int effectiveShields = Shields * 2; // Shields have double resistance to gunfire
                    power -= effectiveShields;
                    if (power > 0)
                    {
                        // Get rid of shields and move down to armor
                        Shields = 0;
                        power -= Armor;
                        if (power > 0)
                        {
                            // Get rid of armor and move down to hull
                            Armor = 0;
                            Hull -= power;
                            if (Hull <= 0)
                            {
                                ShouldDie = true;
                            }
                        }
                        else
                        {
                            Armor -= power;
                        }
                    }
                    else
                    {
                        Shields -= power / 2;
                    }
                    break;
                case DamageType.ENERGY:
                    // Energy weapons are doubly effective against shields but extra damage does not cascade
                    if (Shields > 0)
                    {
                        Shields -= power * 2;
                        if (Shields < 0)
                        {
                            Shields = 0;
                        }
                    }
                    else
                    {
                        // Energy weapons half as effective against armor
                        int effectiveArmor = Armor * 2;
                        power -= effectiveArmor;
                        if (power > 0)
                        {
                            // Get rid of armor and move down to hull
                            Armor = 0;
                            Hull -= power;
                            if (Hull <= 0)
                            {
                                ShouldDie = true;
                            }
                        }
                        else
                        {
                            Armor -= power / 2;
                        }
                    }
                    break;
                case DamageType.EXPLOSIVE:
                    // Explosives are slow-moving and ignored by shields
                    power -= Armor;
                    if (power > 0)
                    {
                        Armor = 0;
                        Hull -= power;
                        if (Hull <= 0)
                        {
                            ShouldDie = true;
                        }
                    }
                    else
                    {
                        Armor -= power;
                    }
                    break;
            }
        }

        internal bool Collide()
        {
            return false;
        }

        internal override void Draw(SpriteBatch batch)
        {
            if (Shields > 0)    Draw_(batch, shieldedTexture);
            else if (Armor > 0) Draw_(batch, armoredTexture);
            else                base.Draw(batch);
        }
    }
}
