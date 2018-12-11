using System;
using System.Collections.Generic;
using Roguelike.Models;
using Roguelike.Controllers;

namespace Roguelike.Services
{
    class MapGenerator
    {
        private static List<char> characters = GenerateProbabilityList();
        private static char[][] map;
        private static Random random = new Random();

        public static char[][] GenerateMap()
        {
            bool nextMapTileGenerated = false;
            CreateMap();
            for (int y = 1; y < map.Length-1; y++)
            {
                map[y][0] = Constants.UnbreakableWallChar;
                for (int x = 1; x < map[y].Length-2; x++)
                {
                    if(y == 1 && x == 1)
                    {
                        continue;
                    }
                    SetCharacter(x,y);

                    switch (map[y][x])
                    {
                        case Constants.TreasureLeftChar:
                            if (x < map[y].Length - 3 || (y == 1 && x == 1))
                            {
                                TreasureChest.TreasureChests.Add(new TreasureChest(x, x + 1, y));
                                x += 1;
                                map[y][x] = Constants.TreasureRightChar;
                            }
                            else
                            {
                                x -= 1;
                            }
                            break;
                        case Constants.MonsterChar:
                            int strength = new Random().Next(1, 3);
                            Monster.monsters.Add(new Monster(strength == 1 ? MonsterType.Weak : MonsterType.Medium, new Position(x, y)));
                            break;
                        case Constants.MonsterStrongChar:
                            Monster.monsters.Add(new Monster(MonsterType.Strong, new Position(x, y)));
                            break;
                        case Constants.BonusLifeChar:
                            new BonusLife(new Random().Next(1, 6), new Position(x,y));
                            break;
                        case Constants.NextLevelChar:
                            if (nextMapTileGenerated)
                            {
                                x -= 1;
                            }
                            else
                            {
                                nextMapTileGenerated = true;
                            }
                            break;

                    }
                }
                map[y][map[y].Length - 2] = Constants.UnbreakableWallChar;
                map[y][map[y].Length - 1] = Constants.NewLine;
            }

            if (!nextMapTileGenerated)
            {
                int y = map.Length - 2;
                int x = map[0].Length - 3;
                Position pos = new Position(x, y);
                switch (map[y][x])
                {
                    case Constants.TreasureRightChar:
                    case Constants.OpenedChestRightChar:
                        // handle chest substitution
                        TreasureChest chest = TreasureChest.TreasureChests.Find(treasure => treasure.ChestRight == pos);
                        if(chest != null)
                        {
                            map[y][chest.ChestLeft.X] = Constants.FreeSpaceChar;
                            map[y][chest.ChestRight.X] = Constants.NextLevelChar;
                            TreasureChest.TreasureChests.Remove(chest);
                        }
                        break;
                    case Constants.MonsterChar:
                    case Constants.MonsterStrongChar:
                        // handle monster substitution
                        Monster enemy = Monster.monsters.Find(monster => monster.position.X == x);
                        if (enemy != null)
                        {
                            map[y][x] = Constants.FreeSpaceChar;
                            map[y][x] = Constants.NextLevelChar;
                            Monster.monsters.Remove(enemy);
                        }
                        break;
                    case Constants.BonusLifeChar:
                        // handle bonus life substitution
                        BonusLife bonus = BonusLife.Bonuses.Find(b => b.Position == pos);
                        if(bonus != null)
                        {
                            map[y][x] = Constants.NextLevelChar;
                            BonusLife.Bonuses.Remove(bonus);
                        }
                        break;
                    default:
                        map[y][x] = Constants.NextLevelChar;
                        break;
                }
            }
            Monster.Start();
            return map;
        }

        private static void AddFullWall()
        {
            for (int i = 0; i < map[0].Length-1; i++)
            {
                map[0][i] = Constants.UnbreakableWallChar;
            }
            map[0][map[0].Length -1] = Constants.NewLine;

            for (int i = 0; i < map[0].Length - 1; i++)
            {
                map[map.Length-1][i] = Constants.UnbreakableWallChar;
            }
            map[map.Length-1][map[0].Length - 1] = Constants.NewLine;
        }

        private static void SetCharacter(int x, int y)
        {
            map[y][x] = characters[random.Next(0, characters.Count)];
        }

        private static void CreateMap()
        {
            map = new char[Constants.MapHeight][];
            for(int i = 0; i < map.Length; i++)
            {
                map[i] = new char[Constants.MapWidth];
            }
            AddFullWall();
        }

        private static List<char> GenerateProbabilityList()
        {
            List<char> currentProbabilityList = new List<char>();

            for(int i = 0; i < Constants.FreeSpaceProbabilitor; i++)
            {
                currentProbabilityList.Add(Constants.FreeSpaceChar);
            }

            for (int i = 0; i < Constants.WallProbabilitor; i++)
            {
                currentProbabilityList.Add(Constants.WallChar);
            }

            for (int i = 0; i < Constants.UnbreakableWallProbabilitor; i++)
            {
                currentProbabilityList.Add(Constants.UnbreakableWallChar);
            }

            for (int i = 0; i < Constants.MonsterProbabilitor; i++)
            {
                currentProbabilityList.Add(Constants.MonsterChar);
            }

            for (int i = 0; i < Constants.StrongMonsterProbabilitor; i++)
            {
                currentProbabilityList.Add(Constants.MonsterStrongChar);
            }

            for (int i = 0; i < Constants.BonusLifeProbabilitor; i++)
            {
                currentProbabilityList.Add(Constants.BonusLifeChar);
            }

            for (int i = 0; i < Constants.TreasureChestProbabilitor; i++)
            {
                currentProbabilityList.Add(Constants.TreasureLeftChar);
            }

            for (int i = 0; i < Constants.NextLevelProbabilitor; i++)
            {
                currentProbabilityList.Add(Constants.NextLevelChar);
            }

            return currentProbabilityList;
        }
    }
}
