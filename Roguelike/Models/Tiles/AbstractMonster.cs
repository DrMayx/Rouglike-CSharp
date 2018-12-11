using Roguelike.Controllers;
using System.Collections.Generic;

namespace Roguelike.Models.Tiles
{
    public abstract class AbstractMonster : TouchableTile, ITile, IInteractable
    {
        public static List<AbstractMonster> Monsters = new List<AbstractMonster>();
        private new string itemName = "Monster";
        public int Power;
        protected int lifes;
        public Position Position;

        public bool IsAlive
        {
            get
            {
                return this.lifes > 0;
            }
        }

        public abstract void DrawCharacter();
        public abstract void Interact(Position pos);

        private void MonsterDie()
        {
            Map.MapTiles[Position.Y][Position.X] = new EmptySpaceTile();
            Game.PlayerRef.Score += this.Power;
            Game.PlayerRef.MonstersKilled++;
            Monsters.Remove(this);
        }

        public void OnReceiveDamage(Position position, int damage)
        {
            if (this.Position == position)
            {
                this.lifes -= damage;
                if (!IsAlive)
                {
                    MonsterDie();
                }
            }
        }

        public void Move()
        {
            // implements AI solution for movement of enemies .. attacking one another following player and stuff like this.
            // TODO !
        }
    }
}
