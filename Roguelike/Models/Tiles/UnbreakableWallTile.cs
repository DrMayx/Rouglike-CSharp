using System;

namespace Roguelike.Models.Tiles
{
    public class UnbreakableWallTile : TouchableTile, ITile
    {
        Position Position;
        public UnbreakableWallTile(Position position)
        {
            this.itemName = "Unbreakable wall";
            this.Position = position;
        }

        public void DrawCharacter()
        {
            Console.Write(Constants.UnbreakableWallChar);
        }
    }
}
