using System;
using System.Threading;
using Roguelike.Models.Tiles;

namespace Roguelike.Controllers
{
    public class MonsterController
    {
        public static bool RefreshingEnabled;
        private static Thread MonsterThread;

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
                    for (int i = 0; i < AbstractMonster.Monsters.Count; i++)
                    {
                        if (AbstractMonster.Monsters[i].IsAlive)
                        {
                            AbstractMonster.Monsters[i].Move();
                        }
                    }
                }
            }
        }

        public static void ClearMonsters()
        {
            RefreshingEnabled = false;
            MonsterThread?.Interrupt();
            MonsterThread = null;
            AbstractMonster.Monsters.Clear();
        }

    }
}
