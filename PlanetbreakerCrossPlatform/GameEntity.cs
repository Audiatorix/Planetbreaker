using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Planetbreaker.Utilities;


namespace Planetbreaker
{
    internal abstract class GameEntity
    {
        internal IHitbox Area { get; private set; }
        protected Texture2D texture;

        private double velx = 0, vely = 0, accx = 0, accy = 0;
        private float rotation;
        public int Layer { get; protected set; }

        public bool ShouldDie { get; protected set; }

        protected GameEntity(IHitbox area, Texture2D texture, int layer, int rotation = 0)
        {
            Area = area;
            this.texture = texture;
            Layer = layer;
            this.rotation = rotation;
        }

        protected void MoveBy(int x, int y)
        {
            Area.MoveBy(x, y);
        }

        protected void MoveTo(int x, int y)
        {
            Area.MoveTo(x, y);
        }

        // dir is in radians
        internal void Accelerate(double acc, double dir)
        {
            accx = acc * Math.Cos(dir);
            accy = acc * Math.Sin(dir);
        }
        internal void Accelerate(double acc, int xpart, int ypart)
        {
            int tot = Math.Abs(xpart) + Math.Abs(ypart);
            accx = acc * (xpart / tot);
            accy = acc * (ypart / tot);
        }

        internal void SetVel(double vel, double dir)
        {
            velx = vel * Math.Cos(dir);
            vely = vel * Math.Sin(dir);
        }

        internal void RotateBy(float radians)
        {
            rotation += radians;
            float twoPi = (float) (2 * Math.PI);
            if (rotation < 0)          rotation += twoPi;
            else if (rotation > twoPi) rotation -= twoPi;
        }

        internal void RotateTo(float radians)
        {
            rotation = radians;
        }

        internal virtual void Draw(SpriteBatch batch)
        {
            Draw_(batch, texture);
        }
        protected void Draw_(SpriteBatch batch, Texture2D texture)
        {
            Rectangle rect = Area.AsRectangle();
            Vector2 pos = new Vector2(rect.Center.X, rect.Center.Y);
            Vector2 origin = new Vector2(rect.Width / 2, rect.Height / 2);
            batch.Draw(texture, pos, null, Color.White, rotation, origin, 1, SpriteEffects.None, 0);
        }

        internal virtual void Update()
        {
            velx += accx;
            vely += accy;
            Area.MoveBy((int) velx, (int) -vely);

            // Die if the entity is too far out
            // TODO do this properly
            Rectangle r = Area.AsRectangle();
            if (r.X < -100 || r.X > 1000 || r.Y < -100 || r.Y > 2000) ShouldDie = true;
        }
    }
}
