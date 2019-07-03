using System;

namespace Monopoly
{
    [Serializable]
    public class PaymentCard:Card
    {
        public decimal Amount { get; set; }
        
        public PaymentCard(string category, string instruction, string description, decimal amount)
            : base(category, instruction, description)
        {
            this.Amount = amount;
        }
        public override string ToString()
        {
            return($"{this.Description} {this.Instruction} ${this.Amount}.");
        }

        public override void ExecuteInstruction(Player player)
        {
            Console.WriteLine($"{player.getName()}: You landed on {this.Category} and drew a card:{this.Description}");

            switch (this.Instruction)
            {
                case "PAY":
                    player.pay(this.Amount);
                    break;
                case "RCV":
                    player.receive(this.Amount);
                    break;
                case "COLLECT":
                    foreach (Player p in Board.access().getPlayers())
                    {
                        if (p.isNotActive() || p == player)
                        {
                            continue;
                        }
                        p.pay(this.Amount);                  
                        player.receive(this.Amount);
                    }
                    break;
                case "PAYALL":
                    foreach (Player p in Board.access().getPlayers())
                    {
                        if (p.isNotActive() || p == player)
                        {
                            continue;
                        }
                        player.pay(this.Amount);
                        if (!player.isNotActive())
                        {
                            p.receive(this.Amount);

                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Configuration error in card.ini!!!");
                    break;

            }

        }
    }
}
