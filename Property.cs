using System;

namespace Monopoly
{

    [Serializable]
    public class Property
    {
        protected string sName;
        protected Trader owner;
        public Property() : this("Property") { }

        public Property(string sName)
        {
            this.sName = sName;
            this.owner = Banker.access();
        }


        public Property(string sName, ref Trader owner)
        {
            this.sName = sName;
            this.owner = owner;
        }
        public Trader getOwner()
        {
            return this.owner;
        }

        
        public void setOwner(Banker newOwner)
        {
            this.owner = newOwner;
        }

        public void setOwner(ref Player newOwner)
        {
            this.owner = newOwner;
        }

        public string getName()
        {
            return this.sName;
        }

        public virtual string landOn(ref Player player)
        {
            return String.Format("{0} landed on {1}. ", player.getName(), this.getName());
        }

        public override string ToString()
        {
            return String.Format("{0,-30}Owned by {1,-12}", this.getName(), this.getOwner().getName());
        }

        public virtual bool availableForPurchase()
        {
            return false;//generic properties are not available for purchase
        }

    }
}
