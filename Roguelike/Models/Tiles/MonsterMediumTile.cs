using System;

namespace Roguelike.Models.Tiles
{
    public class MonsterMediumTile : AbstractMonster
    {
        public MonsterMediumTile(int power, Position position)
        {
            lifes = power;
            Power = power;
            Position = position;
            PlayerTile.CauseDamage += OnReceiveDamage;
            itemName = "Medium monster";
            Monsters.Add(this);
        }

        public override void DrawCharacter()
        {
            Console.ForegroundColor = Constants.StrongMonsterColor;
            Console.Write(Constants.MonsterChar);
            Console.ResetColor();
        }

        public override void Interact(Position pos)
        {
            return;
        }
    }
}
