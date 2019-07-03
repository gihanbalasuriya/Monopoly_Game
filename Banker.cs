using System;
namespace Monopoly
{
 
    [Serializable]
    public class Banker : Trader
    {
        static Banker banker;

        public Banker()
        {
            this.setName("Banker");
           
            this.setBalance(Board.access().BankInitialBalance);

        }

        public static Banker access()
        {
            if (banker == null)
                banker = new Banker();
            return banker;
        }
    }
}
