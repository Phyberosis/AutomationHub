using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutomationHub.com;

namespace AutomationHub
{
    public class ComFactory
    {
        public static ICom getDefaultCom()
        {
            return new ComUSB();
        }
    }
}
