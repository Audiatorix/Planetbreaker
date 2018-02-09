using Microsoft.Xna.Framework;


namespace Planetbreaker.Utilities
{
    internal interface IHitbox
    {
        bool DetectCollision(IHitbox other);
        Rectangle AsRectangle();
        void MoveBy(int x, int y);
        void MoveTo(int x, int y);
    }
}
