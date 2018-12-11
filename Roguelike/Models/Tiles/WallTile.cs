using System;

namespace Roguelike.Models.Tiles
{
    public class WallTile : TouchableTile, ITile
    {
        Position Position;
        public WallTile(Position position)
        {
            this.itemName = "Wall";
            this.Position = position;
        }

        public void DrawCharacter()
        {
            Console.Write(Constants.WallChar);
        }
    }
}
