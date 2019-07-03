using System;
namespace Monopoly
{
    [Serializable]
    public class Card
    {
        public string Category { set; get; }       
        public string Instruction { set; get; }     
        public string Description { set; get; }    

        public Card(string category,string instruction,string description)
        {
            Category = category;
            Instruction = instruction;
            Description = description;
        }
        public override string ToString()
        {
            return ($"{Category} Card: {Description}");
        }

        public virtual void ExecuteInstruction(Player player)
        {
            Console.WriteLine(this.ToString());
        }
    }
}
