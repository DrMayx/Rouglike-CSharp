using Roguelike.Controllers;
using System;
using System.Collections.Generic;

namespace Roguelike.Models.Tiles
{
    public class BonusLifeTile : ITile, IInteractable
    {

        public static List<BonusLifeTile> Bonuses = new List<BonusLifeTile>();
        private int value;
        public Position Position { get; private set; }

        public BonusLifeTile(int value, Position position)
        {
            this.value = value;
            this.Position = position;
            Bonuses.Add(this);
        }

        public void DrawCharacter()
        {
            Console.ForegroundColor = Constants.BonusLifeColor;
            Console.Write(Constants.BonusLifeChar);
            Console.ResetColor();
        }

        public void Interact(Position pos)
        {
            Game.PlayerRef.AddLifes(this.value);
            string properLifeString = this.value == 1 ? "life" : "lifes";
            GameMessage.SendMessage("INFO : You picked up " + this.value + " bonus " + properLifeString);
        }

        public static void ClearBonuses()
        {
            Bonuses.Clear();
        }
    }
}
