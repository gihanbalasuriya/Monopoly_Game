using System;
using System.Collections.Generic;
using System.Text;

namespace Monopoly
{
    public class UtilityFactory : PropertyFactory
    {
        public new Utility create(string sName)
        {
            return new Utility(sName);
        }
    }
}
