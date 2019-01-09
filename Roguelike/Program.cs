using System;
using System.Runtime.InteropServices;
using Roguelike.Controllers;
using Roguelike.Services;
using Roguelike.Models;
using System.Collections.Generic;
using System.Threading;

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
        public static List<Thread> threads = new List<Thread>();

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
                Console.WriteLine("Welcome to Rougelike by DrMayx!\n\n" +
                    "Close game only from main menu.\n" +
                    "Game saves automatically on closing." +
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
            bool isCommand = false;
            Console.WriteLine("Debugging mode. Exit by pressing enter.");
            string command = Console.ReadLine();
            // Debug indicator End

            // Show info for debuging.
            // write debug code here with comments what it checks \/
            do
            {
                switch (command)
                {
                    case "st":
                        isCommand = true;
                        int i = 0;
                        foreach (Thread t in threads)
                        {
                            Console.WriteLine(i++ + t.Name + "\t\t" + t.IsAlive);
                        }
                        break;
                    default:
                        return;
                }
                command = Console.ReadLine();
            } while (isCommand);
        }
    }
}
