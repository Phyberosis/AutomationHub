using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace AutomationHub
{
    public struct ArmData
    {
        public ArmData(int Servos)
        {
            servoAngles = new Angle[Servos];
            for(int i = 0; i < Servos; i++)
            {
                servoAngles[i] = 0;
            }
            position = Vector3.Zero;
            direction = -Vector3.UnitZ;

            servoCount = Servos;
        }

        public ArmData(ArmData d)
        {
            servoAngles = new Angle[d.servoAngles.Length];
            for(int i = 0; i< d.servoAngles.Length; i++)
            {
                servoAngles[i] = new Angle(d.servoAngles[i]);
            }
            position = d.position;
            direction = d.direction;

            servoCount = d.servoCount;
        }

        public readonly int servoCount;

        public Angle[] servoAngles;
        public Vector3 position;
        public Vector3 direction;
    }

    public abstract class Arm
    {
        protected ArmData data;
        protected float grip;

        public Arm()
        {
            data = new ArmData(7);
        }

        public abstract void setPose(Vector3 position, Vector3 direction);

        public ArmData getData() { return new ArmData(data);  }
        public Vector3 getPosition() => data.position;
        public Vector3 getDirection() => data.direction;

        public void setGrip(float percent)
        {
            data.servoAngles[0] = percent * (float)Math.PI * 2f;
        }
        public float getGrip() => (float)data.servoAngles[0];

        public void setServoAngles(Angle[] currentAngles)
        {
            for(int i = 0; i<currentAngles.Length; i++)
            {
                data.servoAngles[i].set(currentAngles[i]);
            }
        }

        public Angle[] getServoAngles() => data.servoAngles;
    }
}
