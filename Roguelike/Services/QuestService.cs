using Roguelike.Controllers;
using Roguelike.Models;
using Roguelike.Models.Tiles;
using System.Threading;

namespace Roguelike.Services
{
    public class QuestService
    {

        public delegate void MissionEventHandler();
        public delegate void QuestUpdtedEventHandler(Quest quest);

        public event MissionEventHandler MissionUpdated;
        public event QuestUpdtedEventHandler QuestUpdated;

        public QuestService()
        {
            Movement.QuestItemInteracted += OnQuestItemInteracted;
        }

        public void OnQuestItemInteracted(IInteractable questItem)
        {
            Quest CurrentQuest = Game.PlayerRef.CurrentQuest;
            if (CurrentQuest != null)
            {
                switch (CurrentQuest.Type)
                {
                    case Quest.QuestType.CollectLives:
                        if(!(questItem is BonusLifeTile)) { return; }
                        CurrentQuest.Progress += 1;
                        QuestUpdated?.Invoke(CurrentQuest);
                        break;
                    case Quest.QuestType.KillMonsters:
                        if(!(questItem is AbstractMonster)) { return; }
                        CurrentQuest.Progress += 1;
                        QuestUpdated?.Invoke(CurrentQuest);
                        break;
                    case Quest.QuestType.FindTreasure:
                        if(!(questItem is TreasureChestTile)) { return; }
                        TreasureChestTile t = (TreasureChestTile)questItem;
                        if (t.IsTreasure)
                        {
                            CurrentQuest.Progress += 1;
                            QuestUpdated?.Invoke(CurrentQuest);
                        }
                        break;
                }
                

                if (CurrentQuest.IsCompleted())
                {
                    Game.PlayerRef.CompleteCurrentQuest();
                }
            }
        }
    }
}
