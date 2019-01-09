using Roguelike.Controllers;
using Roguelike.Models.Tiles;
using System;

namespace Roguelike.Models
{
    public class PlayerTile : ITile
    {
        public delegate void PlayerActionHandler(Position position, int damage);
        public delegate void PlayerDeathHandler();
        public delegate void QuestEvent(Quest quest);
        public delegate void NeedsRefreshHandler();

        public static event PlayerActionHandler CauseDamage;
        public event PlayerDeathHandler PlayerDied;
        public event QuestEvent QuestUpdated;
        public event NeedsRefreshHandler NeedsRefresh;

        public int Score;
        public int Lifes;
        public int MonstersKilled;
        private int attackPower;
        public Position Position;
        public Quest CurrentQuest;

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
            return LoadPlayer(0, 10, 0, null);
        }

        public static PlayerTile LoadPlayer(int score, int lifes, int monsters, Quest currentQuest)
        {
            Instance = new PlayerTile(score, lifes, monsters);
            Instance.CurrentQuest = currentQuest;
            return Instance;
        }

        private PlayerTile(int score, int lifes, int monsters)
        {
            this.Score = score;
            this.Lifes = lifes;
            this.MonstersKilled = monsters;
            this.attackPower = 1;
            this.Position = new Position(1, 1);
            Game.QuestControlActivated += OnQuestControlActivated;
        }

        public void AddLifes(int value)
        {
            if(CurrentQuest != null && CurrentQuest.Type == Quest.QuestType.CollectLives)
            {
                CurrentQuest.Progress += 1;
                QuestUpdated?.Invoke(this.CurrentQuest);

                if(CurrentQuest.IsCompleted())
                {
                    this.Score += this.CurrentQuest.Reward;
                    this.CurrentQuest = null;
                }
            }
            this.Lifes += value;
        }

        public void ExecuteAnyButtonClickedOperations()
        {
            if(CurrentQuest == null || CurrentQuest.IsCompleted())
            {
                CurrentQuest = null;
                QuestUpdated?.Invoke(null);
            }
        }

        private void OnQuestControlActivated(bool isAccepted)
        {
            Console.Write("\b");
            QuestGiver questGiver = QuestGiver.Quests.Find(q => q.IsTouched);
            if (questGiver == null)
            {
                return;
            }

            if (isAccepted)
            {
                GameMessage.SendMessage("ACCEPTED !");
                this.CurrentQuest = questGiver.OwnQuest;
                QuestUpdated?.Invoke(this.CurrentQuest);
            }
            else
            {
                GameMessage.SendMessage("DECLINED !");
            }
            NeedsRefresh?.Invoke();
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
