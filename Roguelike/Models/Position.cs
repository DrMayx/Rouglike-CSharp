namespace Roguelike.Models
{
    public struct Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static bool operator==(Position a, Position b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator!=(Position a, Position b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Position operator+(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsValid()
        {
            return this.X >= 0 && this.Y >= 0;
        }
    }
}
