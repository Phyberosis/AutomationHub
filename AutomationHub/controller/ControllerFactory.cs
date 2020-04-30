using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutomationHub.controllers;
using AHcoms;

namespace AutomationHub
{
    public class ControllerFactory
    {
        public static Controller makeDefaultController(Com com)
        {
            return new ControllerKeyboard(com);
        }
    }
}
