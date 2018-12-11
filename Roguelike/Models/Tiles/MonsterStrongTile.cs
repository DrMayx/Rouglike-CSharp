using System;

namespace Roguelike.Models.Tiles
{
    public class MonsterStrongTile : AbstractMonster
    {
        public MonsterStrongTile(int power, Position position)
        {
            lifes = power;
            Power = power;
            Position = position;
            PlayerTile.CauseDamage += OnReceiveDamage;
            itemName = "Strong monster";
            Monsters.Add(this);
        }

        public override void DrawCharacter()
        {
            Console.ForegroundColor = Constants.StrongMonsterColor;
            Console.Write(Constants.MonsterStrongChar);
            Console.ResetColor();
        }

        public override void Interact(Position pos)
        {
            return;
        }
    }
}
