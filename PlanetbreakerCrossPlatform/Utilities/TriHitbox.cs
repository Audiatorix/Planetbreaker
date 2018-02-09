using Microsoft.Xna.Framework;


namespace Planetbreaker.Utilities
{
    internal class TriHitbox : IHitbox
    {
        private Point r, s, t;
        public Point R
        {
            get { return r; }
            private set { r = value; }
        }
        public Point S
        {
            get { return s; }
            private set { s = value; }
        }
        public Point T
        {
            get { return t; }
            private set { t = value; }
        }

        private int minX, minY, maxX, maxY;

        internal TriHitbox(Point r, Point s, Point t)
        {
            R = r;
            S = s;
            T = t;

            minX = R.X;
            minY = R.Y;
            maxX = R.X;
            maxY = R.Y;

            if (S.X < minX)      minX = S.X;
            else if (S.X > maxX) maxX = S.X;
            if (S.Y < minY)      minY = S.Y;
            else if (S.Y > maxY) maxY = S.Y;

            if (T.X < minX)      minX = T.X;
            else if (T.X > maxX) maxX = T.X;
            if (T.Y < minY)      minY = T.Y;
            else if (T.Y > maxY) maxY = T.Y;
        }

        private struct Line
        {
            internal Point a, b;

            internal Line(Point a, Point b)
            {
                this.a = a;
                this.b = b;
            }

            internal Line(int ax, int ay, int bx, int by)
            {
                a = new Point(ax, ay);
                b = new Point(bx, by);
            }
        }

        public bool DetectCollision(IHitbox other)
        {
            if (other is RectHitbox otherR)
            {
                if (ContainsPoint(new Point(otherR.X1, otherR.X2))) return true;

                Line[] rectLines =
                {
                    new Line(otherR.X1, otherR.Y1, otherR.X1, otherR.Y2),
                    new Line(otherR.X1, otherR.Y1, otherR.X2, otherR.Y1),
                    new Line(otherR.X2, otherR.Y2, otherR.X1, otherR.Y2),
                    new Line(otherR.X2, otherR.Y2, otherR.X2, otherR.Y1),
                };
                Line[] triLines =
                {
                    new Line(R, S),
                    new Line(S, T),
                    new Line(T, R)
                };

                foreach (Line l in rectLines)
                {
                    foreach (Line l2 in triLines)
                    {
                        if (LinesIntersect(l.a, l.b, l2.a, l2.b)) return true;
                    }
                }
            }
            else if (other is TriHitbox otherT)
            {
                if (ContainsPoint(otherT.R)) return true;

                Line[] tri1Lines =
                {
                    new Line(R, S),
                    new Line(S, T),
                    new Line(T, R)
                };
                Line[] tri2Lines =
                {
                    new Line(otherT.R, otherT.S),
                    new Line(otherT.S, otherT.T),
                    new Line(otherT.T, otherT.R)
                };

                foreach (Line l in tri1Lines)
                {
                    foreach (Line l2 in tri2Lines)
                    {
                        if (LinesIntersect(l.a, l.b, l2.a, l2.b)) return true;
                    }
                }
            }

            return false;
        }

        public Rectangle AsRectangle()
        {
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public void MoveBy(int x, int y)
        {
            r.X += x;
            s.X += x;
            t.X += x;
            r.Y += y;
            s.Y += y;
            t.Y += y;

            minX += x;
            maxX += x;
            minY += y;
            maxY += y;
        }

        public void MoveTo(int x, int y)
        {
            MoveBy(x - minX, y - minY);
        }

        private static bool LinesIntersect(Point a1, Point a2, Point b1, Point b2)
        {
            Point cmp   = new Point(b1.X - a1.X, b1.Y - a1.Y);
            Point r     = new Point(a2.X - a1.X, a2.Y - a1.Y);
            Point s     = new Point(b2.X - b1.X, b2.Y - b1.Y);

            int cmpxr = cmp.X * r.Y - cmp.Y * r.X;
            if (cmpxr == 0f)
            {
                // Lines are collinear, and so intersect if they have any overlap
                return ((b1.X - a1.X < 0) != (b1.X - a2.X < 0))
                    || ((b1.Y - a1.Y < 0) != (b1.Y - a2.Y < 0));
            }

            int rxs = r.X * s.Y - r.Y * s.X;
            if (rxs == 0) return false; // Lines are parallel.

            int cmpxs = cmp.X * s.Y - cmp.Y * s.X;
            float rxsr = 1 / rxs;
            float t = cmpxs * rxsr;
            float u = cmpxr * rxsr;

            return (t >= 0f) && (t <= 1f) && (u >= 0f) && (u <= 1f);
        }

        private bool ContainsPoint(Point p)
        {
            int
                dx      = p.X - T.X,
                dy      = p.Y - T.X,
                dx_ts   = T.X - S.X,
                dy_st   = R.Y - T.Y,
                d       = dy_st * (R.X - T.X) + dx_ts * (R.Y - T.Y),
                s       = dy_st * dx + dx_ts * dy,
                t       = (T.Y - R.Y) * dx + (R.X - T.X) * dy;

            if (d < 0) return s <= 0 && t <= 0 && s + t >= d;
            return s >= 0 && t >= 0 && s + t <= d;
        }
    }
}
