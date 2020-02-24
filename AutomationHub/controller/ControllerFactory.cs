using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutomationHub.controller;

namespace AutomationHub
{
    public class ControllerFactory
    {
        public static Controller makeDefaultController(ICom com)
        {
            return new ControllerKeyboard(com);
        }
    }
}
