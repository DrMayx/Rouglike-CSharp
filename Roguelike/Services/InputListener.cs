using System;
using System.Threading;

namespace Roguelike.Services
{

    public class InputListener
    {
        public delegate void dgButtonClicked(ConsoleKeyInfo button);
        public event dgButtonClicked ButtonClicked;

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

            listenerThread.Name = "Input listener Thread";
            listenerThread.Start();
        }

        public void StopListening()
        {
            this.listenerThread?.Interrupt();
            this.isListening = false;
        }
    }
}
