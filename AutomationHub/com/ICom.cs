using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using AutomationHub.com;

namespace AutomationHub
{
    public interface ICom
    {
        Angle[] getCurrentServoAngles();
        void flushPose(ArmData data);
        void send(string msg);
    }
}
