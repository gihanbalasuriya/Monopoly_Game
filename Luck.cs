using System;

namespace Monopoly
{

    [Serializable]
    public class Luck : Property
    {
        bool isBenefitNotPenalty;
        decimal penaltyOrBenefitAmount;

        public Luck() : this("Luck Property", true, 50) { }

        public Luck(string sName, bool isBenefitNotPenalty, decimal amount)
        {
            this.sName = sName;
            this.isBenefitNotPenalty = isBenefitNotPenalty;
            this.penaltyOrBenefitAmount = amount;

        }
        public override string ToString()
        {
            return base.ToString();
        }

        public override string landOn(ref Player player)
        {
            if (this.isBenefitNotPenalty)
            {
                player.receive(this.penaltyOrBenefitAmount);
                return base.landOn(ref player) + String.Format("{0} has recieved {2}.", player.getName(), this.getName(), this.penaltyOrBenefitAmount);
            }
            else
            {
                player.pay(this.penaltyOrBenefitAmount);
                return base.landOn(ref player) + String.Format("{0} has paid {2}.", player.getName(), this.getName(), this.penaltyOrBenefitAmount);
            }
        }
    }
}
