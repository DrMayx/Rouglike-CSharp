using System.Collections.Generic;

namespace Roguelike.Models
{
    class BonusLife
    {
        public static List<BonusLife> Bonuses = new List<BonusLife>();
        public int Value;
        public Position Position;

        public BonusLife(int value, Position position)
        {
            this.Value = value;
            this.Position = position;
            Bonuses.Add(this);
        }
    }
}
