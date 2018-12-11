using Roguelike.Models.Tiles;

namespace Roguelike.Models
{
    public class Map
    {
        public static ITile[][] MapTiles { get; private set; }

        public static void CreateMap()
        {
            MapTiles = new ITile[Constants.MapHeight][];
            for (int i = 0; i < MapTiles.Length; i++)
            {
                MapTiles[i] = new ITile[Constants.MapWidth];
            }
            AddFullWall();
        }

        private static void AddFullWall()
        {
            for (int i = 0; i < MapTiles[0].Length - 1; i++)
            {
                MapTiles[0][i] = new UnbreakableWallTile(new Position(i, 0));
            }
            MapTiles[0][MapTiles[0].Length - 1] = new EmptyNewLineTile();

            for (int i = 0; i < MapTiles[0].Length - 1; i++)
            {
                MapTiles[MapTiles.Length - 1][i] = new UnbreakableWallTile(new Position(i, MapTiles.Length-1));
            }
            MapTiles[MapTiles.Length - 1][MapTiles[0].Length - 1] = new EmptyNewLineTile();
        }
    }
}
