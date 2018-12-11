using System;
using System.Collections.Generic;
using System.Text;

namespace Roguelike.Models.Tiles
{
    class EmptyNewLineTile : ITile
    {
        public void DrawCharacter()
        {
            Console.Write(Constants.NewLine);
        }
    }
}
