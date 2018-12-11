namespace Roguelike.Models.Tiles
{
    public class TouchableTile : ITouchable
    {
        protected string itemName = "Touchable Item";

        public void Touch()
        {
             GameMessage.SendMessage($"INFO: You touched {itemName}");
        }
    }
}
