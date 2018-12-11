using System;

namespace Roguelike.Models.Tiles
{
    public class NextLevelTile : ITile
    {
        Position Position;
        public NextLevelTile(Position position)
        {
            this.Position = position;
        }

        public void DrawCharacter()
        {
            Console.ForegroundColor = Constants.NextLevelTileColor;
            Console.Write(Constants.NextLevelChar);
            Console.ResetColor();
        }
    }
}
