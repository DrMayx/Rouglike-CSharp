using System;
using System.Collections.Generic;
using System.Text;

namespace Roguelike.Models
{
    class TreasureChest
    {
        private readonly static int MaxTreasureAmount = 50;
        public static List<TreasureChest> TreasureChests { get; set; } = new List<TreasureChest>();

        public Position ChestLeft { get; private set; }
        public Position ChestRight { get; private set; }
        public int Treasure { get; private set; }
        public bool IsOpened = false;

        public TreasureChest(int left, int right, int y)
        {
            this.ChestLeft = new Position(left,y);
            this.ChestRight = new Position(right,y);
            this.Treasure = new Random().Next(1, MaxTreasureAmount);
        }
    }
}
