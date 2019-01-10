using Roguelike.Controllers;
using Roguelike.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Roguelike.Models.Tiles
{
    class TreasureChestTile : TouchableTile, ITile, IInteractable
    {
        public static List<TreasureChestTile> TreasureChests { get; set; } = new List<TreasureChestTile>();

        public Position ChestLeft { get; private set; }
        public Position ChestRight { get; private set; }
        public int Treasure { get; private set; }
        public bool IsOpened;
        private bool isLeftDrawn;
        public bool IsTreasure;

        public TreasureChestTile(int left, int right, int y)
        {
            
            this.ChestLeft = new Position(left,y);
            this.ChestRight = new Position(right,y);
            this.Treasure = new Random().Next(Constants.MinTreasureAmount, Constants.MaxTreasureAmount);
            this.IsOpened = false;
            this.isLeftDrawn = false;
            this.itemName = "Treasure chest";
            if (TreasureChests.Find(t => t.IsTreasure) == null)
            {
                this.IsTreasure = this.Treasure % 37 == 0;
                if (IsTreasure)
                {
                    this.Treasure = 0;
                    ErrorDisplayService.ShowError(new Exception(""), $"{left}:{y}", true);
                }
            }
            TreasureChests.Add(this);
        }

        public void Interact(Position pos)
        {
            if (IsOpened)
            {
                Touch();
                return;
            }
            Game.PlayerRef.Score += this.Treasure; // todo fix player event
            this.IsOpened = true;
            if (this.IsTreasure)
            {
                GameMessage.SendMessage("MISSION INFO: You found the hidden treasure!");
                return;
            }
            string properPointsString = this.Treasure == 1 ? "point" : "points";
            GameMessage.SendMessage("INFO : You picked up " + this.Treasure + " bonus " + properPointsString);
        }

        public void DrawCharacter()
        {
            char properCharacter;
            if (this.isLeftDrawn)
            {
                if (this.IsOpened)
                {
                    properCharacter = Constants.OpenedChestRightChar;
                }
                else
                {
                    properCharacter = Constants.TreasureRightChar;
                }
            }
            else
            {
                if (this.IsOpened)
                {
                    properCharacter = Constants.OpenedChestLeftChar;
                }
                else
                {
                    properCharacter = Constants.TreasureLeftChar;
                }
            }
            this.isLeftDrawn = !this.isLeftDrawn;
            Console.Write(properCharacter);
        }
    }
}
