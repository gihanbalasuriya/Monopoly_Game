namespace Monopoly
{
     public class PropertyFactory
    {
         public Property create(string name)
         {
             return new Property(name);
         }
    }
}
