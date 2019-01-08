using System;
using System.Collections.Generic;
using System.Text;

namespace Roguelike.Models
{
    public class GameSave
    {
        public int Score { get; private set; }
        public int Monsters { get; private set; }
        public int Lifes { get; private set; }

        public GameSave(int score, int monsters, int lifes)
        {
            this.Score = score;
            this.Monsters = monsters;
            this.Lifes = lifes;
        }
    }
}
