using System;
using Roguelike.Models;
using Roguelike.Services;

namespace Roguelike.Controllers
{
    public class Movement
    {
        public delegate void MapChanger(char[][] map);
        public event MapChanger MapReloaded;


        private char[][] _map;
        private Position _playerPosition;
        public Movement(ref char[][] map)
        {
            this._map = map;
            this._playerPosition = new Position(1, 1);
        }

        public void Move(int x, int y)
        {
            int xPos = this._playerPosition.X + x;
            int yPos = this._playerPosition.Y + y;
            Position pos = new Position(xPos, yPos);

            if (PositionIsValid(xPos,yPos))
            {
                if (this._map[yPos][xPos] == Constants.TreasureLeftChar)
                {
                    TreasureChest chest = TreasureChest.TreasureChests.Find(item => item.ChestLeft == pos);
                    Game.PlayerRef.Score += chest.Treasure;
                    this._map[yPos][xPos] = Constants.OpenedChestLeftChar;
                    this._map[yPos][xPos + 1] = Constants.OpenedChestRightChar;
                    string properPointsString = chest.Treasure == 1 ? "point" : "points";
                    GameMessage.SendMessage("INFO : You picked up " + chest.Treasure + " bonus " + properPointsString);
                }
                else if (this._map[yPos][xPos] == Constants.TreasureRightChar)
                {
                    TreasureChest chest = TreasureChest.TreasureChests.Find(item => item.ChestRight == pos);
                    Game.PlayerRef.Score += chest.Treasure;
                    this._map[yPos][xPos] = Constants.OpenedChestRightChar;
                    this._map[yPos][xPos - 1] = Constants.OpenedChestLeftChar;
                    string properPointsString = chest.Treasure == 1 ? "point" : "points";
                    GameMessage.SendMessage("INFO : You picked up " + chest.Treasure + " bonus " + properPointsString);
                }
                else if (this._map[yPos][xPos] == Constants.NextLevelChar)
                {
                    LoadNewMap();
                }
                else
                {
                    if (this._map[yPos][xPos] == Constants.BonusLifeChar)
                    {
                        Position futurePos = new Position(xPos, yPos);
                        BonusLife bonus = BonusLife.Bonuses.Find(b => b.Position == futurePos);
                        Game.PlayerRef.Lifes += bonus.Value;
                        string properLifeString = bonus.Value == 1 ? "life" : "lifes";
                        GameMessage.SendMessage("INFO : You picked up " + bonus.Value + " bonus " + properLifeString);
                    }
                    this._map[yPos][xPos] = Constants.PlayerChar;
                    this._map[this._playerPosition.Y][this._playerPosition.X] = Constants.FreeSpaceChar;
                    this._playerPosition.X += x;
                    this._playerPosition.Y += y;
                }
            }
            else
            {
                string itemName = GetItemName(yPos,xPos);
                GameMessage.SendMessage($"INFO: You touched {itemName}");
            }

            MonsterAttackOnPlayerProximity(yPos, xPos);
        }

        private void LoadNewMap()
        {
            Monster.ClearMonsters();
            this._map = MapGenerator.GenerateMap();
            MapReloaded?.Invoke(this._map);
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

            Monster monster = null;
            foreach(Position p in positionsToCheck)
            {
                try
                {
                    monster = Monster.monsters.Find(m => m.position + p == this._playerPosition);
                    if(monster != null)
                    {
                        break;
                    }
                }
                catch (ArgumentNullException e)
                {
                    Game.ShowError(e);
                    return;
                }
            }

            if(monster == null)
            {
                return;
            }

            if (monster.Type == MonsterType.Weak || monster.Type == MonsterType.Medium)
            {
                if(monster.position + positionsToCheck[0] == this._playerPosition
                    || monster.position + positionsToCheck[1] == this._playerPosition
                    || monster.position + positionsToCheck[2] == this._playerPosition 
                    || monster.position + positionsToCheck[3] == this._playerPosition)
                {
                    return;
                }
            }

            int monsterDamage = (int)monster.Type;

            // weak = 25% chance     -||    range is {0,1,2,3}  ]
            // medium = 33% chance   -||    range is {0,1,2}    ]---- condition is only true for 0
            // strong = 50% chance   -\/    range is {0,1}      ]
            int damage = new Random().Next(0, Constants.MosterDamageChanceProbablitor - monsterDamage);
            if (damage % Constants.MosterDamageChanceProbablitor == 0)
            {
                GameMessage.SendMessage("ATTACKED! : " + monster.Type + " monster hit you and\ncaused " + monsterDamage + " damage to You!");
                Game.PlayerRef.ApplyDamage(monsterDamage);
            }
        }

        private string GetItemName(int y, int x)
        {
            switch (this._map[y][x])
            {
                case Constants.MonsterChar:
                case Constants.MonsterStrongChar:
                    return "Monster";
                case Constants.OpenedChestLeftChar:
                case Constants.OpenedChestRightChar:
                    return "Opened chest";
                case Constants.TreasureLeftChar:
                case Constants.TreasureRightChar:
                    return "Closed chest";
                case Constants.UnbreakableWallChar:
                    return "Unbreakable wall";
                case Constants.WallChar:
                    return "Wall";
                default:
                    return "Undefined";
            }
        }

        private bool PositionIsValid(int xPos, int yPos)
        {
            return this._map[yPos][xPos] != Constants.WallChar
                && this._map[yPos][xPos] != Constants.UnbreakableWallChar
                && this._map[yPos][xPos] != Constants.OpenedChestLeftChar
                && this._map[yPos][xPos] != Constants.OpenedChestRightChar
                && this._map[yPos][xPos] != Constants.MonsterStrongChar
                && this._map[yPos][xPos] != Constants.MonsterChar;
        }

        public void Attack(int x, int y)
        {
            int yPos = this._playerPosition.Y + y;
            int xPos = this._playerPosition.X + x;

            switch (this._map[yPos][xPos])
            {
                case Constants.FreeSpaceChar:
                    break;
                case Constants.WallChar:
                    this._map[yPos][xPos] = Constants.FreeSpaceChar;
                    Game.PlayerRef.Score += 1;
                    break;
                case Constants.MonsterChar:
                case Constants.MonsterStrongChar:
                    Monster monster = null;
                    Position attackPos = this._playerPosition + new Position(x, y);
                    try
                    {
                        monster = Monster.monsters.Find(item => item.position == attackPos);
                    }
                    catch (ArgumentNullException)
                    {
                        // swallow it.. will be checked in next if ... thou it shouldnt happen theoritically
                    }
                    if(monster != null)
                    {
                        Player.Attack(attackPos);
                    }
                    MonsterAttackOnPlayerProximity(yPos, xPos);
                    break;
            }
        }
    }
}
