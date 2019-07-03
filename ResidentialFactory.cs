namespace Monopoly
{
    public class ResidentialFactory : PropertyFactory
    {
        public Residential create(string sName, decimal dPrice, decimal dRent, decimal dHouseCost, decimal dHotelCost)
        {
            return new Residential(sName, dPrice, dRent, dHouseCost, dHotelCost);
        }
    }
}
