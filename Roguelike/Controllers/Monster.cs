using System;
using System.Collections.Generic;
using System.Threading;
using Roguelike.Models;

namespace Roguelike.Controllers
{
    public enum MonsterType
    {
        Weak = 1, Medium = 2, Strong = 3
    }
    public class Monster
    {
        public static List<Monster> monsters = new List<Monster>();
        public int Lifes;
        public int Damage;
        public MonsterType Type;
        public bool IsAlive
        {
            get
            {
                return Lifes > 0;
            }
        }
        public static bool RefreshingEnabled;
        private static Thread MonsterThread;
        public Position position;

        public Monster(MonsterType type, Position pos)
        {
            Type = type;
            Lifes = (int) Type;
            Damage = (int) Type;
            position = pos;
            monsters.Add(this);
            Player.CauseDamage += OnReceiveDamage;
        }

        public static void Start()
        {
            MonsterThread = new Thread(Update);
            MonsterThread.Name = "Monster update thread";
            RefreshingEnabled = true;
            MonsterThread.Start();
        }

        private static void Update()
        {
            while (RefreshingEnabled)
            {
                if (DateTime.Now.Millisecond % 500 == 0)
                {
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        if (monsters[i].IsAlive)
                        {
                            monsters[i].Move();
                        }
                    }
                }
            }
        }

        private void Move()
        {
            // implements AI solution for movement of enemies .. attacking one another following player and stuff like this.
            // TODO !
        }

        private void MonsterDie()
        {
            Game.Map[position.Y][position.X] = Constants.FreeSpaceChar;
            Game.PlayerRef.Score += (int)this.Type;
            Game.PlayerRef.MonstersKilled++;
            monsters.Remove(this);
        }

        public void OnReceiveDamage(Position position, int damage)
        {
            if(this.position == position)
            {
                this.Lifes -= damage;
                if (!IsAlive)
                {
                    MonsterDie();
                }
            }
        }

        public static void ClearMonsters()
        {
            RefreshingEnabled = false;
            MonsterThread?.Interrupt();
            MonsterThread = null;
            monsters.Clear();
        }

    }
}
