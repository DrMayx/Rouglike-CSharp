using System;
using System.Runtime.InteropServices;
using Roguelike.Controllers;
using Roguelike.Models;

// started 3:35 14.11.2018              // Started project
// przerwa 3:52 - 4:10                  // Added basic random map generation
// przerwa 5:30 - 15.11.2018 2:22       // Added movement
// przerwa 4:08 - 15:20                 // debugging session - multithreading issues
// przerwa 16:03 - 18.11.2018 13:07     // Added basic attack - now only walls, Added unbreakable walls, improved map generation
// przerwa 13:44 - 13:50                // Added treasure chests
// przerwa 14:10 - 19.11.2018 5:00      // Added basic ui for score, lives and monster killed statistics
// przerwa 7:20 - 21.11.2018  4:20      // Added monsters - weak medium and strong, added player damage statistic, 
                                        // Added colors for monsters and player, added continue option, added killed monsters counter
// przerwa 4:41 - 4:45                  // Added GameMessage; Fix error with thread disappearing on game exit
// przerwa 6:25 - 23.11.2018 3:30       // Added not yet well working monster attacking player on sight; Added game finishing on players death
// przerwa 4:00 - 5:20                  // Fixed monster attack on sight, added bonus lifes providing additional lives through the gameplay
// przerwa 5:25 - 13:00                 // Added messages on monster hit, on treasure chest open and on bonus life picked up
// przerwa 19:00 -                      // REFACTOR - whole game made to use tiles instead if characters

namespace Roguelike
{
    class Program
    {   
        #region Some data for window unresizability
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        #endregion

        private static Game game = null;
        private static PlayerTile player = null;

        static void Main(string[] args)
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle != IntPtr.Zero)
            {
                DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            }

            bool userAtMenu = true;
            while (userAtMenu)
            {
                if (game != null)
                {
                    continue;
                }
                bool playerToPlay = ShowMenu();
                if (playerToPlay)
                {
                    game = new Game(player);
                    game.GameFinished += OnGameFinished;
                    game.Run();
                }
                else
                {
                    userAtMenu = false;
                }
            }
        }

        public static void OnGameFinished()
        {
            game = null;
        }

        public static bool ShowMenu()
        {
            bool atMenu = true;
            while (atMenu)
            {
                Console.Clear();
                bool continueCondition = player != null && player.IsAlive;
                string continueOption = continueCondition ? "\n[2]Continue" : "";
                Console.WriteLine($"Welcome to Rougelike by DrMayx!\n\n[1]Start new Game{continueOption}\n[9]Exit");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        player = PlayerTile.CreateNewPlayer();
                        return true;
                    case ConsoleKey.D2:
                        if(continueCondition)
                        {
                            return true;
                        }
                        break;
                    case ConsoleKey.D9:
                        return false;
                    case ConsoleKey.D0:
                        ShowDebugInfo();
                        break;
                }
            }
            return false;
        }

        private static void ShowDebugInfo()
        {
            // Show info for debuging.
            // write debug code here with comments what it checks \/


            // Don't remove this statement it is for clear reading purposes;
            Console.ReadLine();
        }
    }
}
