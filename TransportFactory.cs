using System;
using System.Collections.Generic;
using System.Text;

namespace Monopoly
{
    public class TransportFactory : ResidentialFactory
    {
        public new Transport create(string sName)
        {
            return new Transport(sName);
        }
    }
}
