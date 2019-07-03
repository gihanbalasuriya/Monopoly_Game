using System;

namespace Monopoly
{
    [Serializable]
    public class RepairCard:Card
    {
        public decimal UnitFeeOfHouse { get; set; }
        public decimal UnitFeeOfHotel { get; set; }

        public RepairCard(string category, string instruction, string description, decimal housefee, decimal hotelfee) 
            :base(category,instruction,description)
        {
            this.UnitFeeOfHouse = housefee;
            this.UnitFeeOfHotel = hotelfee;
        }

        public override void ExecuteInstruction(Player player)
        {
            Console.WriteLine($"{player.getName()}: You landed on {this.Category} and drew a card:{this.Description}");
            decimal totalFee = 0.00m;
            foreach (Property p in player.getPropertiesOwnedFromBoard())
            {
                if (p.GetType() == (new Residential().GetType()))
                {
                    totalFee+=((Residential)p).getHouseCount()*this.UnitFeeOfHouse;
                }

            }
            if (Math.Abs(totalFee)>0.05m)
            {
                player.pay(totalFee);
            }

        }
    }
}
