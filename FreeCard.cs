using System;

namespace Monopoly
{
    [Serializable]
    public class FreeCard:Card
    {
        public FreeCard(string category, string instruction, string description) : base(category, instruction, description) { }

        public override void ExecuteInstruction(Player player)
        {
            Console.WriteLine($"{player.getName()}: You landed on {this.Category} and drew a card:{this.Description}");
            if ("Chance"==this.Category)
            {
                player.HasChanceFreeCard = true;

            }
            else
            {
                player.HasChestFreeCard = true;

            }
        }

        public bool TradeFreeCard(Player seller, Player buyer, decimal amount)
        {
            if (!seller.HasChanceFreeCard && !seller.HasChestFreeCard)
            {
                Console.WriteLine($"{seller.getName()} has no free card!!!");
                return false;
            }
            if (buyer.getBalance()>amount)
            {
                buyer.pay(amount);
                seller.receive(amount);
                if (seller.HasChanceFreeCard)
                {
                    seller.HasChanceFreeCard = false;
                    buyer.HasChanceFreeCard = true;
                }
                else
                {
                    seller.HasChestFreeCard = false;
                    buyer.HasChestFreeCard = true;

                }
                return true;
            }
            else
            {
                Console.WriteLine($"{buyer.getName()} has no enough money!!!");
                return false;

            }



        }

    }
}
