using System;
using System.Collections.Generic;

namespace Roguelike.Models.Tiles
{
    public class QuestGiver : ITile, IInteractable
    {
        public static List<QuestGiver> Quests = new List<QuestGiver>();
        public bool IsTouched
        {
            get
            {
                Position[] positionsToCheck = new Position[]
                {
                    new Position(-1, 0),
                    new Position(0, -1),
                    new Position(0, 1),
                    new Position(1, 0),
                };

                Position p;
                foreach(Position position in positionsToCheck)
                {
                    p = this.Position + position;
                    if(Map.MapTiles[p.Y][p.X] is PlayerTile)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public Position Position;
        public Quest OwnQuest;


        public QuestGiver(Position position)
        {
            this.Position = position;
            this.OwnQuest = new Quest();
            Quests.Add(this);

        }

        public void DrawCharacter()
        {
            Console.BackgroundColor = Constants.QuestGiverColor;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(Constants.QuestGiverChar);
            Console.ResetColor();
        }

        public void Interact(Position pos)
        {
            GameMessage.SendMessage($"NEW QUEST!\n{this.OwnQuest.Type}\n[O] Accept\n[P] Decline");
        }

        public static void ClearQuests()
        {
            Quests.Clear();
        }
    }
}
