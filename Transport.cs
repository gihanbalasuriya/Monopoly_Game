using System;

namespace Monopoly
{
    [Serializable]
    public class Transport : TradableProperty
    {

        public Transport() : this("Railway Station"){}

        public Transport(string sName)
        {
            this.sName = sName;
            this.dPrice = 200;
            this.dMortgageValue = 100;
            this.dRent = 50;
            this.owner = Banker.access();
        }

        public override string ToString()
        {
            return base.ToString() + string.Format("\t{0,12}", this.IsMortgaged ? "Mortgaged" : "Unmortgaged");
        }
    }
}
