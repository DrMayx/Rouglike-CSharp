using System;
using System.Threading;
using Roguelike.Models;
using Roguelike.Services;

namespace Roguelike.Controllers
{
    public class Game
    {
        public delegate void GameHandler();
        public event GameHandler GameFinished;

        public static Player PlayerRef;
        private Thread GameThread;
        private InputListener inputListener;
        public volatile bool isPlaying;
        public static char[][] Map;
        private Movement movement;
        private bool isAttacking;
        public static bool IsError;
        private bool _hasMoved;
        private string[] _messages = null;
        private int _lines = 0;
        public static Game Instance;

        public Game(Player player)
        {
            PlayerRef = player;
            Instance = this;
        }

        public void Run()
        {
            GameThread = new Thread(Start);
            GameThread.Name = "Game thread";
            inputListener = new InputListener();
            GameThread.Start();
        }

        private void Start()
        {
            _hasMoved = false;
            Map = MapGenerator.GenerateMap();
            inputListener.ButtonClicked += OnButtonClicked;
            inputListener.StartListening();
            PlayerRef.PlayerDied += OnPlayerDied;
            movement = new Movement(ref Map);
            movement.MapReloaded += OnMapReloaded;
            PutPlayerOnMap();
            isPlaying = true;
            GameMessage.NewMessageOccured += OnNewMessageOccured;

            while (isPlaying)
            {
                if (DateTime.Now.Millisecond % 500 == 0)
                {
                    ShowMap();
                }
            }
        }

        public void OnMapReloaded(char[][] map)
        {
            Map = map;
            this.movement = new Movement(ref Map);
            this.movement.MapReloaded += OnMapReloaded;
            PutPlayerOnMap();
        }

        public void ShowMap()
        {
            if (!isPlaying) { return; }
            Console.Clear();
            for (int rowIdx = 0; rowIdx < Map.Length; rowIdx++)
            {
                for(int colIdx = 0; colIdx < Map[rowIdx].Length; colIdx++)
                {
                    try
                    {
                        if (colIdx == Map[rowIdx].Length-1)
                        {
                            PrintUI(rowIdx);
                        }
                        else
                        {
                            DrawCharacter(rowIdx,colIdx);
                        }
                    }catch(ThreadInterruptedException e)
                    {
                        ShowError(e);
                        return;
                    }
                }
            }
        }

        public static void ShowError(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("An error occured. But you can continue playing.\nFor debugging info read error message below, else just press Enter and play!\n\n");
            Console.WriteLine(e);
            Console.ResetColor();
            IsError = true;
            Game.Instance.isPlaying = false;
        }

        private void DrawCharacter(int rowIdx, int colIdx)
        {
            if (Map[rowIdx][colIdx] == Constants.MonsterChar || Map[rowIdx][colIdx] == Constants.MonsterStrongChar)
            {
                Monster monster = null;
                try
                {
                    monster = Monster.monsters.Find(item => item.position == new Position(colIdx, rowIdx));
                }
                catch (ArgumentNullException)
                {
                    // ...
                }
                if(monster != null)
                {
                    switch (monster.Type)
                    {
                        case MonsterType.Weak:
                            Console.ForegroundColor = Constants.WeakMonsterColor;
                            break;
                        case MonsterType.Medium:
                        case MonsterType.Strong:
                            Console.ForegroundColor = Constants.StrongMonsterColor;
                            break;
                    }
                }
            }

            else if(Map[rowIdx][colIdx] == Constants.PlayerChar)
            {
                Console.ForegroundColor = Constants.PlayerColor;
            }
            else if(Map[rowIdx][colIdx] == Constants.BonusLifeChar)
            {
                Console.ForegroundColor = Constants.BonusLifeColor;
            }
            else if(Map[rowIdx][colIdx] == Constants.NextLevelChar)
            {
                Console.ForegroundColor = Constants.NextLevelTileColor;
            }
            Console.Write(Map[rowIdx][colIdx]);
            Console.ResetColor();
        }

        private void PrintUI(int rowIdx)
        {
            switch (rowIdx)
            {
                case 2:
                    Console.Write("\t\t\tScore: " + PlayerRef.Score + "\n");
                    break;
                case 4:
                    Console.Write("\t\t\tLifes: " + PlayerRef.Lifes + "\n");
                    break;
                case 6:
                    Console.Write("\t\t\tMonsters killed: " + PlayerRef.MonstersKilled + "\n");
                    break;
                default:
                    PrintGameMessage(rowIdx);
                    break;
            }
        }

        private void PrintGameMessage(int row)
        {
            if (_hasMoved)
            {
                _lines = 0;
                _messages = null;
            }

            int currentRow = row - 8;
            if (_lines > 0 && currentRow >= 0 && currentRow < _messages.Length)
            {
                Console.Write("\t\t\t" + _messages[currentRow] + "\n");
            }
            else
            {
                Console.Write("\n");
            }
        }

        public void PutPlayerOnMap()
        {
            Map[1][1] = Constants.PlayerChar;
        }

        public void OnPlayerDied()
        {
            Console.Clear();
            Console.WriteLine("You Died!");
            GameStop();
        }

        public void OnNewMessageOccured(string[] messages, int lines)
        {
            _hasMoved = false;
            _lines = lines;
            _messages = messages;
        }

        public void OnButtonClicked(ConsoleKeyInfo button)
        {
            _hasMoved = true;
            if (IsError)
            {
                isPlaying = false;
                GameFinished?.Invoke();
                inputListener.StopListening();
                GameThread.Interrupt();
                Monster.RefreshingEnabled = false;
                return;
            }
            switch (button.Key)
            {
                case ConsoleKey.Escape:
                    Console.Clear();
                    GameStop();
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (isAttacking)
                    {
                        movement.Attack(-1,0);
                        isAttacking = false;
                        break;
                    }
                    movement.Move(-1,0);
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (isAttacking)
                    {
                        movement.Attack(1, 0);
                        isAttacking = false;
                        break;
                    }
                    movement.Move(1,0);
                    break;
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (isAttacking)
                    {
                        movement.Attack(0, -1);
                        isAttacking = false;
                        break;
                    }
                    movement.Move(0,-1);
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (isAttacking)
                    {
                        movement.Attack(0, 1);
                        isAttacking = false;
                        break;
                    }
                    movement.Move(0,1);
                    break;
                case ConsoleKey.Spacebar:
                    isAttacking = true;
                    break;
            }
        }

        public void GameStop()
        {
            Console.WriteLine("Your score: " + PlayerRef.Score);
            Console.WriteLine("Press any key to continue...");
            inputListener.StopListening();
            isPlaying = false;
            Monster.ClearMonsters();
            Console.ReadKey();
            GameFinished?.Invoke();
            GameThread.Interrupt();
        }
    }
}
