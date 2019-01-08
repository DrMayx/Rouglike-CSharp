using System;
using System.Threading;
using Roguelike.Models;
using Roguelike.Models.Tiles;
using Roguelike.Services;

namespace Roguelike.Controllers
{
    public class Game
    {
        public delegate void GameHandler();
        public event GameHandler GameFinished;
        public delegate void QuestControl(bool ifAccpted);
        public static event QuestControl QuestControlActivated;

        public static PlayerTile PlayerRef;
        private Thread GameThread;
        private InputListener inputListener;
        public volatile bool isPlaying;
        private Movement movement;
        private bool isAttacking;
        public static bool IsError;
        private bool _hasMoved;
        private string[] _messages = null;
        private int _lines = 0;
        public static Game Instance;

        public Game(PlayerTile player)
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
            MapGenerator.GenerateMap();
            inputListener.ButtonClicked += OnButtonClicked;
            inputListener.StartListening();
            PlayerRef.PlayerDied += OnPlayerDied;
            PlayerRef.QuestUpdated += OnQuestUpdated;
            movement = new Movement();
            movement.PlayerMoved += ShowMap;
            movement.PlayerAttacked += ShowMap;
            movement.NeedRefresh += PrintMap;
            isPlaying = true;
            GameMessage.NewMessageOccured += OnNewMessageOccured;

            PrintMap();
        }

        public void ShowMap(Position targetPosition)
        {
            if (PlayerRef.IsAlive)
            {
                if (targetPosition.IsValid())
                {
                    Console.SetCursorPosition(PlayerRef.Position.X, PlayerRef.Position.Y);
                    Map.MapTiles[PlayerRef.Position.Y][PlayerRef.Position.X].DrawCharacter();
                    Console.SetCursorPosition(targetPosition.X, targetPosition.Y);
                    Map.MapTiles[targetPosition.Y][targetPosition.X].DrawCharacter();
                }

                Console.SetCursorPosition(Constants.MapWidth, 2);
                PrintUI(2);
                Console.SetCursorPosition(Constants.MapWidth, 4);
                PrintUI(4);
                Console.SetCursorPosition(Constants.MapWidth, 6);
                PrintUI(6);
                HandleMessages();

                Console.SetCursorPosition(0, Constants.MapHeight);
            }
        }

        public void PrintMap()
        {
            if (!isPlaying) { return; }
            Console.Clear();
            for (int rowIdx = 0; rowIdx < Map.MapTiles.Length; rowIdx++)
            {
                for (int colIdx = 0; colIdx < Map.MapTiles[rowIdx].Length; colIdx++)
                {
                    try
                    {
                        if (colIdx == Map.MapTiles[rowIdx].Length - 1)
                        {
                            PrintUI(rowIdx);
                        }
                        else
                        {
                            DrawCharacter(rowIdx, colIdx);
                        }
                    }
                    catch (ThreadInterruptedException e)
                    {
                        ShowError(e);
                        return;
                    }
                }
            }
            HandleQuestArea(PlayerRef.CurrentQuest);
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
            if (Map.MapTiles[rowIdx][colIdx] == null)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write("-");
                Console.ResetColor();
            }
            else
            {
                Map.MapTiles[rowIdx][colIdx].DrawCharacter();
            }
        }

        private void PrintUI(int rowIdx)
        {
            switch (rowIdx)
            {
                case 2:
                    Console.Write("\t\t\tScore: " + PlayerRef.Score + "\t\n");
                    break;
                case 4:
                    Console.Write("\t\t\tLifes: " + PlayerRef.Lifes + "\t\n");
                    break;
                case 6:
                    Console.Write("\t\t\tMonsters killed: " + PlayerRef.MonstersKilled + "\t\n");
                    break;
                default:
                    Console.Write("\n");
                    break;
            }
        }

        public void HandleMessages()
        {
            ClearMessageArea();
            _messages = _hasMoved ? null : _messages;

            if (_messages == null)
            {
                return;
            }

            for(int i = 0; i < _messages.Length; i++)
            {
                Console.SetCursorPosition(Constants.MapWidth-1, Constants.MessageAreaStartRow + i);
                Console.Write(" \t\t\t" + _messages[i] + "\n");
            }
        }

        public void ClearMessageArea()
        {
            for (int currentRow = 0; currentRow < Constants.MessageAreaHeight; currentRow++)
            {
                Console.SetCursorPosition(Constants.MapWidth - 1, Constants.MessageAreaStartRow + currentRow);

                for (int currentCol = Constants.MapWidth -1; currentCol < Constants.WindowWidth-1; currentCol++)
                {
                    Console.Write(" ");
                }
                Console.Write("\n");
            }
        }

        public void ClearQuestArea()
        {
            for (int currentRow = 0; currentRow < Constants.QuestAreaHeight; currentRow++)
            {
                Console.SetCursorPosition(Constants.MapWidth - 1, Constants.QuestAreaStartRow + currentRow);

                for (int currentCol = Constants.MapWidth - 1; currentCol < Constants.WindowWidth - 1; currentCol++)
                {
                    Console.Write(" ");
                }
                Console.Write("\n");
            }
        }

        public void HandleQuestArea(Quest quest)
        {
            if (quest == null)
            {
                return;
            }

            Console.SetCursorPosition(Constants.MapWidth - 1, Constants.QuestAreaStartRow);
            Console.Write("\t\t\t________QUEST________");
            Console.SetCursorPosition(Constants.MapWidth - 1, Constants.QuestAreaStartRow + 2);
            Console.Write("\t\t\tType: " + quest.Type);
            Console.SetCursorPosition(Constants.MapWidth - 1, Constants.QuestAreaStartRow + 3);
            Console.Write("\t\t\tProgress: " + quest.Progress + "/" + quest.Value);
            Console.SetCursorPosition(Constants.MapWidth - 1, Constants.QuestAreaStartRow + 4);
            Console.Write("\t\t\tReward: " + quest.Reward);
            Console.SetCursorPosition(0, Constants.MapHeight);
        }

        public void PrintQuestCompletedMessage(Quest quest)
        {
            Console.SetCursorPosition(Constants.MapWidth - 1, Constants.QuestAreaStartRow);
            Console.Write("\t\t\t\tQUEST FINISHED!");
            Console.SetCursorPosition(Constants.MapWidth - 1, Constants.QuestAreaStartRow + 2);
            Console.Write($"\t\t\tYou are rewarded with {quest.Reward} points!");
        }

        public void OnQuestUpdated(Quest quest)
        {
            if(quest == null)
            {
                PrintMap();
                return;
            }
            if(quest.Progress == quest.Value)
            {
                ClearQuestArea();
                PrintQuestCompletedMessage(quest);
            }
            else{
                HandleQuestArea(quest);
            }
        }

        public void OnPlayerDied()
        {
            SaveLoadService.RemoveSaveFile();
            Console.Clear();
            Console.WriteLine("You Died!");
            GameStop();
        }

        public void OnNewMessageOccured(string[] messages, int lines)
        {
            _hasMoved = false;
            _lines = lines;
            _messages = messages;
            HandleMessages();
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
                MonsterController.RefreshingEnabled = false;
                return;
            }
            switch (button.Key)
            {
                case ConsoleKey.Escape:
                    Console.Clear();
                    SaveLoadService.Save(PlayerRef);
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
                case ConsoleKey.O:
                    QuestControlActivated?.Invoke(true);
                    break;
                case ConsoleKey.P:
                    QuestControlActivated?.Invoke(false);
                    break;
            }
        }

        public void GameStop()
        {
            Console.WriteLine("Your score: " + PlayerRef.Score);
            Console.WriteLine("Press any key to continue...");
            inputListener.StopListening();
            isPlaying = false;
            MonsterController.ClearMonsters();
            Console.ReadKey();
            GameFinished?.Invoke();
            GameThread.Interrupt();
        }
    }
}
