using System;

namespace Roguelike.Models.Tiles
{
    public class EmptySpaceTile : ITile
    {
        public void DrawCharacter()
        {
            Console.Write(Constants.FreeSpaceChar);
        }
    }
}
