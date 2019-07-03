using System;


namespace Monopoly
{

    [Serializable]
    public class Dice
    {
        private static Random numGenerator = new Random();
        private int numberRolled;
        
        public int roll()
        {
            numberRolled = numGenerator.Next(1, 7);
            return numberRolled;
        }

        public int numberLastRolled()
        {
            return numberRolled;
        }
         
        public override string ToString()
        {
            return numberRolled.ToString();
        }
    }
}
