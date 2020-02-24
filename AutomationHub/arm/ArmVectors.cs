using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHub.arm
{
    internal class ArmVectors : Arm
    {
        public override void setPose(Vector3 position, Vector3 direction)
        {
            data.position = position;
            data.direction = direction;
        }
    }
}
