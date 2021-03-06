﻿using System;
using Roguelike.Models;
using Roguelike.Models.Tiles;
using Roguelike.Services;

namespace Roguelike.Controllers
{
    public class Movement
    {
        public delegate void QuestItemInteractedEventHandler(IInteractable item);
        public delegate void MoveEvent(Position target);
        public delegate void TouchEvent();

        public event MoveEvent PlayerMoved;
        public event MoveEvent PlayerAttacked;
        public event TouchEvent NeedRefresh;
        public event TouchEvent ItemTouched;
        public static event QuestItemInteractedEventHandler QuestItemInteracted;

        public Movement()
        {
            AbstractMonster.MonsterDied += OnMonsterDied;
        }

        public void Move(int x, int y)
        {

            int xPos = Game.PlayerRef.Position.X + x;
            int yPos = Game.PlayerRef.Position.Y + y;
            int oldPlayerPosX = int.Parse(Game.PlayerRef.Position.X.ToString());
            int oldPlayerPosY = int.Parse(Game.PlayerRef.Position.Y.ToString());
            Position pos = new Position(xPos, yPos);
            Position previousPos = new Position(oldPlayerPosX, oldPlayerPosY);

            if (PositionIsValid(xPos,yPos))
            {
                if (Map.MapTiles[yPos][xPos] is TreasureChestTile)
                {
                    TreasureChestTile chest = TreasureChestTile.TreasureChests.Find(item => item.ChestRight == pos || item.ChestLeft == pos);
                    if (chest != null && chest.IsOpened)
                    {
                        chest.Touch();
                        ItemTouched?.Invoke();
                    }
                    chest.Interact(pos);
                    NeedRefresh?.Invoke();
                    QuestItemInteracted?.Invoke(chest);
                }
                else if (Map.MapTiles[yPos][xPos] is NextLevelTile)
                {
                    LoadNewMap();
                }
                else if (Map.MapTiles[yPos][xPos] is QuestGiver)
                {
                    QuestGiver quest = QuestGiver.Quests.Find(q => q.Position == pos);
                    quest.Interact(pos);
                }
                else
                {
                    if (Map.MapTiles[yPos][xPos] is BonusLifeTile)
                    {
                        Position futurePos = new Position(pos.X, pos.Y);
                        BonusLifeTile bonus = BonusLifeTile.Bonuses.Find(b => b.Position == futurePos);
                        bonus.Interact(pos);
                        QuestItemInteracted?.Invoke(bonus);
                    }
                    Map.MapTiles[yPos][xPos] = Game.PlayerRef;
                    Map.MapTiles[Game.PlayerRef.Position.Y][Game.PlayerRef.Position.X] = new EmptySpaceTile();
                    Game.PlayerRef.Position.X += x;
                    Game.PlayerRef.Position.Y += y;
                }
            }
            else
            {
                if(Map.MapTiles[yPos][xPos] is ITouchable)
                {
                    ((ITouchable)Map.MapTiles[yPos][xPos]).Touch();
                }
            }

            MonsterAttackOnPlayerProximity(yPos, xPos);
            PlayerMoved?.Invoke(previousPos);
        }

        private void LoadNewMap()
        {
            QuestGiver.ClearQuests();
            BonusLifeTile.ClearBonuses();
            MonsterController.ClearMonsters();
            MapGenerator.GenerateMap();
            NeedRefresh?.Invoke();
        }

        private void MonsterAttackOnPlayerProximity(int y, int x)
        {
            Position[] positionsToCheck = new Position[]
            {
                new Position(-1, -1),       // strong
                new Position(1, 1),         // strong
                new Position(1, -1),        // strong
                new Position(-1, 1),        // strong
                new Position(-1, 0),
                new Position(0, -1),
                new Position(0, 1),
                new Position(1, 0),
            };

            AbstractMonster monster = null;
            foreach(Position p in positionsToCheck)
            {
                try
                {
                    monster = AbstractMonster.Monsters.Find(m => m.Position + p == Game.PlayerRef.Position);
                    if(monster != null)
                    {
                        break;
                    }
                }
                catch (ArgumentNullException e)
                {
                    ErrorDisplayService.ShowError(e);
                    return;
                }
            }

            if(monster == null)
            {
                return;
            }

            if (monster is MonsterMediumTile || monster is MonsterWeakTile)
            {
                if(monster.Position + positionsToCheck[0] == Game.PlayerRef.Position
                    || monster.Position + positionsToCheck[1] == Game.PlayerRef.Position
                    || monster.Position + positionsToCheck[2] == Game.PlayerRef.Position 
                    || monster.Position + positionsToCheck[3] == Game.PlayerRef.Position)
                {
                    return;
                }
            }

            int monsterDamage = monster.Power;

            // weak = 25% chance     -||    range is {0,1,2,3}  ]
            // medium = 33% chance   -||    range is {0,1,2}    ]---- condition is only true for 0
            // strong = 50% chance   -\/    range is {0,1}      ]
            int damage = new Random().Next(0, Constants.MosterDamageChanceProbablitor - monsterDamage);
            string monsterType = monster is MonsterWeakTile ? "Weak" : monster is MonsterMediumTile ? "Medium" : "Strong";
            if (damage % Constants.MosterDamageChanceProbablitor == 0)
            {
                GameMessage.SendMessage($"ATTACKED! : {monsterType} monster hit you and\ncaused {monsterDamage} damage to You!");
                Game.PlayerRef.ApplyDamage(monsterDamage);
            }
        }

        private bool PositionIsValid(int xPos, int yPos)
        {
            return !(Map.MapTiles[yPos][xPos] is WallTile)
                && !(Map.MapTiles[yPos][xPos] is UnbreakableWallTile)
                && !(Map.MapTiles[yPos][xPos] is AbstractMonster);
        }

        public void Attack(int x, int y)
        {
            int yPos = Game.PlayerRef.Position.Y + y;
            int xPos = Game.PlayerRef.Position.X + x;

            int oldPlayerPosX = int.Parse(Game.PlayerRef.Position.X.ToString());
            int oldPlayerPosY = int.Parse(Game.PlayerRef.Position.Y.ToString());
            Position previousPos = new Position(oldPlayerPosX, oldPlayerPosY);


            if (Map.MapTiles[yPos][xPos] is EmptySpaceTile)
            {
                return;
            }
            else if(Map.MapTiles[yPos][xPos] is WallTile)
            {
                Map.MapTiles[yPos][xPos] = new EmptySpaceTile();
                Game.PlayerRef.Score += 1;
            }
            else if(Map.MapTiles[yPos][xPos] is AbstractMonster)
            {
                AbstractMonster monster = null;
                Position attackPos = Game.PlayerRef.Position + new Position(x, y);
                monster = AbstractMonster.Monsters.Find(item => item.Position == attackPos);
                if (monster != null)
                {
                    Game.PlayerRef.Attack(attackPos);
                }
                MonsterAttackOnPlayerProximity(yPos, xPos);
            }
            else
            {
                return;
            }

            PlayerAttacked?.Invoke(new Position(xPos, yPos));
        }

        public static void OnMonsterDied(AbstractMonster monster)
        {
            QuestItemInteracted?.Invoke(monster);
        }

        public void ClearDependencies()
        {
            AbstractMonster.MonsterDied -= OnMonsterDied;
        }
    }
}
