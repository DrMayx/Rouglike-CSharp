using System;

namespace Roguelike.Services
{
    public class ErrorDisplayService
    {
        public static void ShowError(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("An error occured. But you can continue playing.\nFor debugging info read error message below, else just press Enter and play!\n\n");
            Console.WriteLine(e);
            Console.ResetColor();
        }

        public static void ShowError(Exception e, string additional, bool pause)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("An error occured. But you can continue playing.\nFor debugging info read error message below, else just press Enter and play!\n\n");
            Console.WriteLine(additional + "\n\n");
            Console.WriteLine(e);
            if (pause)
            {
                Console.ReadLine();
            }
            Console.ResetColor();
        }
    }
}
