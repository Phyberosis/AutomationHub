using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHdata
{
    public abstract class Instructions
    {

        public static Instructions create(Angle[] arr)
        {
            return new InstructionsAngle(arr);
        }

        public static Instructions create(KeyFrame k)
        {
            return new InstructionsKeyFrame(k);
        }
    }

    internal class InstructionsKeyFrame: Instructions
    {
        private KeyFrame dFrame;

        internal InstructionsKeyFrame(KeyFrame frame)
        {
            dFrame = frame;
        }
    }

    internal class InstructionsAngle : Instructions
    {
        private Angle[] dAngles;

        internal InstructionsAngle(Angle[] arr)
        {
            dAngles = arr;
        }
    }
}
