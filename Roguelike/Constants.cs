using System;

namespace Roguelike
{
    public class Constants
    {
        // Display settings
        public const int RightMargin = 26;
        public const int MapWidth = 50;
        public const int MapHeight = 22;

        /*

            Constants.MonsterChar, Constants.MonsterChar,
            Constants.MonsterStrongChar,
            Constants.BonusLifeChar,
            Constants.NextLevelChar
             */


        // Probabilitors
        public const int MosterDamageChanceProbablitor = 5;
        public const int FreeSpaceProbabilitor = 800;
        public const int WallProbabilitor = 256;
        public const int UnbreakableWallProbabilitor = 32;
        public const int MonsterProbabilitor = 32;
        public const int StrongMonsterProbabilitor = 16;
        public const int TreasureChestProbabilitor = 32;
        public const int BonusLifeProbabilitor = 16;
        public const int NextLevelProbabilitor = 1;

        // Element textures
        public const char WallChar = '#';
        public const char FreeSpaceChar = ' ';
        public const char NewLine = '\n';
        public const char PlayerChar = '@';
        public const char UnbreakableWallChar = '8';
        public const char TreasureLeftChar = '[';
        public const char TreasureRightChar = ']';
        public const char OpenedChestLeftChar = '{';
        public const char OpenedChestRightChar = '}';
        public const char MonsterChar = 'x';
        public const char MonsterStrongChar = 'X';
        public const char BonusLifeChar = 'O';
        public const char NextLevelChar = '&';

        // Element colors
        public const ConsoleColor PlayerColor = ConsoleColor.Green;
        public const ConsoleColor WeakMonsterColor = ConsoleColor.DarkRed;
        public const ConsoleColor StrongMonsterColor = ConsoleColor.Red;
        public const ConsoleColor BonusLifeColor = ConsoleColor.Yellow;
        public const ConsoleColor NextLevelTileColor = ConsoleColor.Magenta;
    }
}
