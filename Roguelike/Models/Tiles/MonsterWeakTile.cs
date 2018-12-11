using System;

namespace Roguelike.Models.Tiles
{
    public class MonsterWeakTile : AbstractMonster
    {
        public MonsterWeakTile(int power, Position position)
        {
            lifes = power;
            Power = power;
            Position = position;
            PlayerTile.CauseDamage += OnReceiveDamage;
            itemName = "Weak monster";
            Monsters.Add(this);
        }

        public override void DrawCharacter()
        {
            Console.ForegroundColor = Constants.WeakMonsterColor;
            Console.Write(Constants.MonsterChar);
            Console.ResetColor();
        }

        public override void Interact(Position pos)
        {
            return;
        }
    }
}
