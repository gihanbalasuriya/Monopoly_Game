using System;

namespace Monopoly
{
    [Serializable]
    public class TradableProperty : Property 
    {
        protected decimal dPrice;
        protected decimal dMortgageValue;
        protected decimal dRent;
        public bool IsMortgaged { get; set; }


        public TradableProperty()
        {
            this.dPrice = 200;
            this.dMortgageValue = 100;
            this.dRent = 50;
        }

        public decimal getPrice()
        {
            return dPrice;
        }

        public virtual decimal getRent()
        {
            return this.dRent;
        }

        public virtual void payRent(ref Player player)
        {
            player.pay(this.getRent());
            this.getOwner().receive(this.getRent());
        }

        public decimal getMortgageValue()
        {
            return this.dMortgageValue;
        }

        public void purchase(ref Player buyer)
        {
            //check that it is owned by bank
            if (this.availableForPurchase())
            {
                //pay price 
                buyer.pay(this.getPrice());
                //set owner to buyer
                this.setOwner(ref buyer);
            }
            else
            {
                throw new ApplicationException("The property is not available from purchase from the Bank.");
            }
        }

        public override bool availableForPurchase()
        {
            //if owned by bank then available
            if (this.owner == Banker.access())
                return true;
            return false;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override string landOn(ref Player player)
        {
            //Pay rent if needed
            if ((this.getOwner() != Banker.access()) && (this.getOwner() != player) && !this.IsMortgaged)
            {
                //pay rent
                Console.WriteLine($">>>>>>>>>>{player.getName()}, you should pay ${this.getRent()} to {this.getOwner().getName()} for landing {this.getName()}<<<<<<<<<<");
                this.payRent(ref player);
                return base.landOn(ref player) + string.Format("Rent has been paid for {0} of ${1} to {2}.", this.getName(), this.getRent(), this.getOwner().getName());
            }
            else
                return base.landOn(ref player);
        }

        public bool mortgage(Player player)
        {
            this.IsMortgaged = true;
            player.receive(this.dMortgageValue);
            return true;
        }

        public bool unmortgage(Player player)
        {
            decimal amount = this.dMortgageValue * 1.1m;
            if (amount > player.getBalance())
            {
                Console.WriteLine($"{ player.getName()}:   You have no enough money to pay unmortgage fee!!!");
                return false;
            }
            this.IsMortgaged = false;
            player.pay(amount);
            return true;
        }
    }
}
