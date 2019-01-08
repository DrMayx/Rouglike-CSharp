using System;
using System.Runtime.InteropServices;
using Roguelike.Controllers;
using Roguelike.Services;
using Roguelike.Models;

// started 3:35 14.11.2018              // Started project
// przerwa 3:52 - 4:10                  // Added basic random map generation
// przerwa 5:30 - 15.11.2018 2:22       // Added movement
// przerwa 4:08 - 15:20                 // debugging session - multithreading issues
// przerwa 16:03 - 18.11.2018 13:07     // Added basic attack - now only walls, Added unbreakable walls, improved map generation
// przerwa 13:44 - 13:50                // Added treasure chests
// przerwa 14:10 - 19.11.2018 5:00      // Added basic ui for score, lives and monster killed statistics
// przerwa 7:20 - 21.11.2018  4:20      // Added monsters - weak medium and strong, added player damage statistic, 
//                                      // Added colors for monsters and player, added continue option, added killed monsters 
//                                      // counter
// przerwa 4:41 - 4:45                  // Added GameMessage; Fix error with thread disappearing on game exit
// przerwa 6:25 - 23.11.2018 3:30       // Added not yet well working monster attacking player on sight; Added game finishing on 
//                                      // players death
// przerwa 4:00 - 5:20                  // Fixed monster attack on sight, added bonus lifes providing additional lives through the 
//                                      // gameplay
// przerwa 5:25 - 13:00                 // Added messages on monster hit, on treasure chest open and on bonus life picked up
// przerwa 19:00 - 7.01.2019 7:10       // REFACTOR - whole game made to use tiles instead if characters
// przerwa 8:24 -  4:30                 // Added persistance. player statistics are encrypted and saved on disk when player exits 
//                                      // a game
// przerwa 6:33 - 7:00                  // OFFICIALLY VERSION 0.1 ALPHA ! 
//                                      // bettered framerate, fixed message system, resolved some concurrency issues with saving 
//                                      // fixed saves not disappearing after player died.
namespace Roguelike
{
    class Program
    {   
        #region Some data for window unresizability
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_SIZE = 0xF000;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_CLOSE = 0xF060;


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
                DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(sysMenu, SC_CLOSE, MF_BYCOMMAND);
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
            if (SaveLoadService.CheckIfGamesaveExists())
            {
                player = SaveLoadService.Load();
            }
            bool atMenu = true;
            Console.Clear();
            while (atMenu)
            {
                Console.Clear();
                bool continueCondition = player != null && player.IsAlive;
                string continueOption = continueCondition ? "\n[2]Continue" : "";
                Console.WriteLine($"Welcome to Rougelike by DrMayx!\n\n" +
                    $"Close game only from main menu.\n" +
                    $"Game saves automatically on closing." +
                    $"\n\n[1]Start new Game{continueOption}\n[9]Exit");

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
            // Do not remove this section
            // Debug indicator Start
            Console.WriteLine("Debugging mode. Exit by pressing enter.");
            string command = Console.ReadLine();
            // Debug indicator End


            // Show info for debuging.
            // write debug code here with comments what it checks \/
        }
    }
}
