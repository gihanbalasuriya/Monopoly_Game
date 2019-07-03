using System;

namespace Monopoly
{
    [Serializable]
    public class Residential : TradableProperty
    {
        
        decimal dHouseCost;
        int iHouses;
        static int iMaxHouses = 4;
        decimal dHotelCost;
        int iHotels;
        static int iMaxHotels = 1;


        public Residential() : this("Residential Property"){}

        public Residential(String sName) : this(sName, 200, 50, 50, 60) { }

        public Residential(String sName, decimal dPrice, decimal dRent, decimal dHouseCost, decimal dHotelCost)
        {
            this.sName = sName;
            this.dPrice = dPrice;
            this.dMortgageValue = dPrice * 0.8m;
            this.dRent = dRent;
            this.dHouseCost = dHouseCost;
            this.dHotelCost = dHotelCost;
            this.IsMortgaged = false;
        }
        
        public override decimal getRent()
        {
            return (dRent + (dRent * iHouses)+(dRent * iHotels * 2));
        }


        public void addHouse()
        {
            this.getOwner().pay(this.dHouseCost);
            this.iHouses ++;
        }

        public void reduceHouse()
        {
            this.getOwner().receive(this.dHouseCost*0.5m);
            this.iHouses--;
        }

        public decimal getHouseCost()
        {
            return this.dHouseCost;
        }

        public int getHouseCount()
        {
            return this.iHouses;
        }

        public static int getMaxHouses()
        {
            return iMaxHouses;
        }

        public void addHotel()
        {
            this.getOwner().pay(this.dHotelCost);
            this.iHotels++;
        }

        public void reduceHotel()
        {
            this.getOwner().receive(this.dHotelCost * 0.5m);
            this.iHotels--;
        }

        public decimal getHotelCost()
        {
            return this.dHotelCost;
        }

        public int getHotelCount()
        {
            return this.iHotels;
        }

        public static int getMaxHotels()
        {
            return iMaxHotels;
        }
        public override string ToString()
        {
           return base.ToString()  + string.Format("\t{0,12}", this.IsMortgaged ? "Mortgaged" : "Unmortgaged") + string.Format("\tHouses: {0}\tHotels:{1}", this.getHouseCount(),this.getHotelCount());
        }        
     }
}
