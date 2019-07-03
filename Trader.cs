using System;
using System.Collections;

namespace Monopoly
{

    [Serializable]
    public class Trader 
    {
        protected ArrayList propertiesOwned = new ArrayList();
        protected decimal dBalance;
        protected string sName;

        public Trader() { }

        public Trader(string sName, decimal dBalance)
        {
            this.sName = sName;
            this.dBalance = dBalance;
        }


        public void receive(decimal dAmount)
        {
            this.dBalance += dAmount;
        }

        public void pay(decimal dAmount)
        {
            this.dBalance -= dAmount;
            checkBankrupt();
        }

        public virtual void checkBankrupt()
        {
            if (this.getBalance() <= 0)
                throw new ApplicationException(String.Format("{0} is Bankrupt", this.getName()));
        }



        public override string ToString()
        {
            return String.Format("Name: {0} \nBalance: {1}", this.sName, this.dBalance);
        }

        public String getName()
        {
            return this.sName;
        }

        public void setName(String sName)
        {
            this.sName = sName;
        }

        public void setBalance(decimal dBalance)
        {
            this.dBalance = dBalance;
        }

        public decimal getBalance()
        {
            return this.dBalance;
        }

        public void obtainProperty(ref Property property)
        {
            this.propertiesOwned.Add(property);
        }

        public void tradeProperty(ref TradableProperty property, ref Player purchaser, decimal amount)
        {
            purchaser.pay(amount);
            this.receive(amount);
            property.setOwner(ref purchaser);
        }

        
        public ArrayList getPropertiesOwned()
        {
            return this.propertiesOwned;
        }


    }
}
