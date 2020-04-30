using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHcoms
{
    public class ComFactory
    {
        public static Com getDefaultCom()
        {
            return new ComLAN();
        }
    }
}
