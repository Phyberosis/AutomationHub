using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutomationHub.arm;

namespace AutomationHub
{
    public class ArmFactory
    {
        public static Arm makeDefaultArm()
        {
            return new ArmVectors();
        }
    }
}
