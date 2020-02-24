using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHub
{
    public class Angle
    {
        private float val;

        private void init(float j)
        {
            val = j;
        }

        public Angle()
        {
            init(0f);
        }

        public Angle(float j)
        {
            init(rectify(j));
        }

        public Angle(Angle j)
        {
            init(j.val);
        }

        public void set(Angle j)
        {
            val = rectify(j.val);
        }

        private float rectify(float j)
        {
            const float PI = (float)Math.PI;
            while (j < 0)
            {
                j += 2 * PI;
            }

            while (j >= 2 * PI)
            {
                j -= 2 * PI;
            }

            return j;
        }

        public float get()
        {
            return val;
        }

        public float toDegrees()
        {
            return ((val * 180f) / (float)Math.PI);
        }

        public static explicit operator float(Angle j) => j.val;
        public static implicit operator Angle(float j)
        {
            return new Angle(j);
        }

        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static Angle operator +(Angle i, Angle j)
        {
            return new Angle(i.val + j.val);
        }

        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static Angle operator -(Angle i, Angle j)
        {
            return new Angle(i.val - j.val);
        }

        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static Angle operator *(Angle i, Angle j)
        {
            return new Angle(i.val * j.val);
        }

        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static Angle operator /(Angle i, Angle j)
        {
            return new Angle(i.val / j.val);
        }
    }
}
