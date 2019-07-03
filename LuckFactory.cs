namespace Monopoly
{
    public class LuckFactory : PropertyFactory
    {
        public Luck create(string sName, bool isPenalty, decimal dAmount)
        {
            return new Luck(sName, isPenalty, dAmount);
        }
    }
}
