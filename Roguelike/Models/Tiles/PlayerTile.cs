using Roguelike.Models.Tiles;
using System;

namespace Roguelike.Models
{
    public class PlayerTile : ITile
    {
        public delegate void PlayerActionHandler(Position position, int damage);
        public delegate void PlayerDeathHandler();
        public static event PlayerActionHandler CauseDamage;
        public event PlayerDeathHandler PlayerDied;

        public int Score;
        public int Lifes;
        public int MonstersKilled;
        private int attackPower;
        public Position Position;

        public bool IsAlive
        {
            get
            {
                return Lifes > 0;
            }
        }
        public static PlayerTile Instance { get; private set; } = null;

        public static PlayerTile CreateNewPlayer()
        {
            if(Instance == null)
            {
                Instance = new PlayerTile();
            }
            return Instance;
        }

        private PlayerTile()
        {
            this.Score = 0;
            this.Lifes = 10;
            this.MonstersKilled = 0;
            this.attackPower = 1;
            this.Position = new Position(1, 1);
        }

        public void Attack(Position pos)
        {
            CauseDamage?.Invoke(pos, attackPower);
        }

        public void ApplyDamage(int damage)
        {
            Lifes -= damage;
            if(Lifes <= 0)
            {
                PlayerDied?.Invoke();
            }
        }

        public void DrawCharacter()
        {
            Console.ForegroundColor = Constants.PlayerColor;
            Console.Write(Constants.PlayerChar);
            Console.ResetColor();
        }
    }
}
