using Microsoft.Xna.Framework;


namespace Planetbreaker.Utilities
{
    internal class RectHitbox : IHitbox
    {
        internal int X1 { get; private set; }
        internal int X2 { get; private set; }
        internal int Y1 { get; private set; }
        internal int Y2 { get; private set; }
        internal int Width { get; private set; }
        internal int Height { get; private set; }

        internal RectHitbox(int x, int y, int width, int height)
        {
            X1 = x;
            Y1 = y;
            X2 = x + width;
            Y2 = y + height;

            Width = width;
            Height = height;
        }

        public bool DetectCollision(IHitbox other)
        {
            if (other is RectHitbox otherR)
            {
                return
                    X1 < otherR.X2 && X2 > otherR.X1 &&
                    Y2 > otherR.Y1 && Y1 < otherR.Y2;
            }
            else if (other is TriHitbox)
            {
                // Defer the work to avoid duplicate code
                return other.DetectCollision(this);
            }

            return false;
        }

        public Rectangle AsRectangle()
        {
            return new Rectangle(X1, Y1, Width, Height);
        }

        public void MoveBy(int x, int y)
        {
            X1 += x;
            Y1 += y;
            X2 += x;
            Y2 += y;
        }

        public void MoveTo(int x, int y)
        {
            X1 = x;
            Y1 = y;
            X2 = x + Width;
            Y2 = y + Height;
        }
    }
}
