using System;

namespace Monopoly
{
    [Serializable]
    public class MovingCard:Card
    {
        public string Destination { get; set; }
        public int Steps { get; set; }

        public MovingCard(string category, string instruction, string description,string destination,int steps=0) 
            :base(category,instruction,description)
        {
            this.Destination = destination;
            this.Steps = steps;
        }
        public override string ToString()
        {
            return($"{this.Description} {this.Instruction} {this.Destination} {this.Steps}");
        }
        public override void ExecuteInstruction(Player player)
        {
            Console.WriteLine($"{player.getName()}: You landed on {this.Category} and drew a card:{this.Description}");
            if (Steps==0 && Destination == null)
            {
                Console.WriteLine($"Configuration of Card:{this.ToString()} error!, Please check the configure file card.ini!!!");
                return;
            }
            if (Steps == 0)
            {
                int destinationIndex = -1;
               
                for(int i=0;i< Board.access().getProperties().Count;i++)
                {
                    if (Destination==((Property)Board.access().getProperties()[i]).getName())
                    {
                        destinationIndex = i;

                        break;
                    }
                }
                if (destinationIndex==-1)
                {
                    Console.WriteLine($"There is no this destination[{Destination}], Please check the configure file Properties.ini and Cards.ini!!!");
                    return;
                }
                else
                {
                    if (this.Destination=="Jail")
                    {
                        player.IsInJail = true;
                        player.DoubleCount = 0;
                    }
                    else if (player.getLocation()> destinationIndex)
                    {
                        destinationIndex =  Board.access().getSquares() + destinationIndex;
                    }
                    player.setLocation(destinationIndex);
                }
            }
            else
            {
                player.setLocation(player.getLocation()+Steps);
            }

        }
    }
}
