using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHub.com
{
    internal class ComUSB : ICom
    {
        public void flushPose(ArmData data)
        {
            //Console.WriteLine("comUSB flush");
        }

        public Angle[] getCurrentServoAngles()
        {
            Angle[] temp = new Angle[7];
            for(int i =0; i<7; i++)
            {
                temp[i] = new Angle();
            }
            return temp;
        }

        public void send(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
