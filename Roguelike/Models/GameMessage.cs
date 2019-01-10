using System;
using System.Collections.Generic;
using System.Text;

namespace Roguelike.Models
{
    public class GameMessage
    {
        public delegate void MessageHamdler(string[] message, int numberOfLines);

        public static event MessageHamdler NewMessageOccured;

        public static void SendMessage(string message)
        {
            string[] messages = message.Split("\n");
            int lines = messages.Length;
            NewMessageOccured?.Invoke(messages, lines);
        }
    }
}
