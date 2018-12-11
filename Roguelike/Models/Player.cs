namespace Roguelike.Models
{
    public class Player
    {
        public delegate void PlayerActionHandler(Position position, int damage);
        public delegate void PlayerDeathHandler();
        public static event PlayerActionHandler CauseDamage;
        public event PlayerDeathHandler PlayerDied;

        public int Score = 0;
        public int Lifes = 10;
        public int MonstersKilled = 0;
        private static int attackPower = 1;
        public bool IsAlive
        {
            get
            {
                return Lifes > 0;
            }
        }

        public static void Attack(Position pos)
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
    }
}
