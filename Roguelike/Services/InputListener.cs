using System;
using System.Threading;

namespace Roguelike.Services
{

    public class InputListener
    {
        public delegate void dgButtonClicked(ConsoleKeyInfo button);
        public event dgButtonClicked ButtonClicked;

        private static int counter = 0;
        private Thread listenerThread;
        private bool isListening = true;

        public void StartListening()
        {
            listenerThread = new Thread(() =>
            {
                while (isListening)
                {
                    if (Console.KeyAvailable)
                    {
                        ButtonClicked(Console.ReadKey());
                    }
                }
            });

            listenerThread.Name = "Input listener Thread " + counter++;
            Program.threads.Add(listenerThread);
            listenerThread.Start();
        }

        public void StopListening()
        {
            this.isListening = false;
            listenerThread = null;
            this.listenerThread?.Abort();
        }
    }
}
