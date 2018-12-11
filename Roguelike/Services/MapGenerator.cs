using System;
using System.Collections.Generic;
using Roguelike.Models;
using Roguelike.Controllers;
using Roguelike.Models.Tiles;

namespace Roguelike.Services
{
    class MapGenerator
    {
        private static List<char> characters = GenerateProbabilityList();
        private static Random random = new Random();
        private static bool nextMapTileGenerated;

        public static void GenerateMap()
        {
            nextMapTileGenerated = false;
            Map.CreateMap();
            Map.MapTiles[1][1] = Game.PlayerRef;
            Game.PlayerRef.Position = Constants.PlayerStartingPosition;
            for (int y = 1; y < Map.MapTiles.Length-1; y++)
            {
                Map.MapTiles[y][0] = new UnbreakableWallTile(new Position(0,y));
                for (int x = 1; x < Map.MapTiles[y].Length-2; x++)
                {
                    if(y == 1 && x == 1)
                    {
                        continue;
                    }
                    SetTile(ref x, y);
                }
                Map.MapTiles[y][Map.MapTiles[y].Length - 2] = new UnbreakableWallTile(new Position(y, Map.MapTiles[y].Length - 2));
                Map.MapTiles[y][Map.MapTiles[y].Length - 1] = new EmptyNewLineTile();
            }

            if (!nextMapTileGenerated)
            {
                int y = Map.MapTiles.Length - 2;
                int x = Map.MapTiles[0].Length - 3;
                Position pos = new Position(x, y);

                if(Map.MapTiles[y][x] is TreasureChestTile)
                {
                    TreasureChestTile chest = TreasureChestTile.TreasureChests.Find(treasure => treasure.ChestRight == pos);
                    if (chest != null)
                    {
                        Map.MapTiles[y][chest.ChestLeft.X] = new EmptySpaceTile();
                        Map.MapTiles[y][chest.ChestRight.X] = new NextLevelTile(pos);
                        TreasureChestTile.TreasureChests.Remove(chest);
                    }
                }
                else if(Map.MapTiles[y][x] is AbstractMonster)
                {
                    AbstractMonster enemy = AbstractMonster.Monsters.Find(monster => monster.Position.X == x);
                    if (enemy != null)
                    {
                        Map.MapTiles[y][x] = new NextLevelTile(pos);
                        AbstractMonster.Monsters.Remove(enemy);
                    }
                }else if(Map.MapTiles[y][x] is BonusLifeTile)
                {
                    BonusLifeTile bonus = BonusLifeTile.Bonuses.Find(b => b.Position == pos);
                    if (bonus != null)
                    {
                        Map.MapTiles[y][x] = new NextLevelTile(pos);

                        BonusLifeTile.Bonuses.Remove(bonus);
                    }
                }
                else
                {
                    Map.MapTiles[y][x] = new NextLevelTile(pos);
                }
            }
            MonsterController.Start();
        }

        private static void SetTile(ref int x, int y)
        {
            char currentCharacter = characters[random.Next(0, characters.Count)];

            ITile tile = null;
            Position position = new Position(x, y);
            switch (currentCharacter)
            {
                case Constants.TreasureLeftChar:
                    if (x < Map.MapTiles[y].Length - 3 || (y == 1 && x == 1))
                    {
                        tile = new TreasureChestTile(x, x + 1, y);
                        Map.MapTiles[y][x] = tile;
                        Map.MapTiles[y][x + 1] = tile;
                        x += 1;
                        return;
                    }
                    else
                    {
                        x -= 1;
                        return;
                    }
                case Constants.MonsterChar:
                    int power = new Random().Next(1, 4);
                    if (power == 2) {
                        tile = new MonsterMediumTile(power, position);
                    }
                    else
                    {
                        tile = new MonsterWeakTile(1, position);
                    }
                    break;
                case Constants.MonsterStrongChar:
                    tile = new MonsterStrongTile(3, position);
                    break;
                case Constants.BonusLifeChar:
                    tile = new BonusLifeTile(new Random().Next(1, 6), position);
                    break;
                case Constants.NextLevelChar:
                    if (nextMapTileGenerated)
                    {
                        x -= 1;
                        return;
                    }
                    else
                    {
                        tile = new NextLevelTile(position);
                        nextMapTileGenerated = true;
                    }
                    break;
                case Constants.FreeSpaceChar:
                    tile = new EmptySpaceTile();
                    break;
                case Constants.WallChar:
                    tile = new WallTile(position);
                    break;
                case Constants.UnbreakableWallChar:
                    tile = new UnbreakableWallTile(position);
                    break;
            }
            Map.MapTiles[y][x] = tile;
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
