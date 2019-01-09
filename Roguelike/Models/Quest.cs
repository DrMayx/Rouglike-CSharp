using System;
using System.Collections.Generic;
using System.Text;

namespace Roguelike.Models
{
    public class Quest
    {
        public enum QuestType
        {
            KillMonsters = 0,
            FindTreasure = 1,
            CollectLives = 2
        }

        public QuestType Type;
        public int Value;
        public int Progress;
        public int Reward;

        public Quest()
        {
            SetQuestType();
            this.Value = this.Type == QuestType.FindTreasure ? 1 : new Random().Next(1, 5);
            this.Progress = 0;
        }

        public Quest(int type, int progress, int value, int reward)
        {
            this.Type = (QuestType) type;
            this.Progress = progress;
            this.Value = value;
            this.Reward = reward;
        }

        private void SetQuestType()
        {
            QuestType type;
            int reward;
            Random random = new Random();

            switch (random.Next(0, 3))
            {
                case 0:
                    type = QuestType.KillMonsters;
                    reward = random.Next(10, 51);
                    break;
                case 1:
                    type = QuestType.FindTreasure;
                    reward = random.Next(5, 21);
                    break;
                case 2:
                    type = QuestType.CollectLives;
                    reward = random.Next(100, 501);
                    break;
                default:
                    type = QuestType.KillMonsters;
                    reward = random.Next(10, 51);
                    break;
            }
            this.Type = type;
            this.Reward = reward;
            if (random.Next(0, 10000) == 72)
            {
                this.Reward = 1337;
            }
            // Debug
            this.Type = QuestType.CollectLives;
        }

        public bool IsCompleted()
        {
            return this.Progress >= this.Value;
        }
    }
}
