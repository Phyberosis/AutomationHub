using AHdata;
using AHcoms;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutomationHub.controllers
{
    internal class ControllerKeyboard : Controller
    {
        private KeyProcessor kpPosDir, kpRaw;
        public ControllerKeyboard(Com com) : base(com)
        {
            kpPosDir = new PosDirProcessor();
            kpRaw = new RawAnglesProcessor();
        }

        protected override bool step(ref ComData d, float dt)
        {
            KeyProcessor keyProcessor = keysToggle.Contains(Key.CapsLock)? kpRaw : kpPosDir;

            float rate = keysToggle.Contains(Key.F)? 10 : 2.5f;

            lock (this)
            {
                return keyProcessor.getNextTarget(ref d, keysDown, keysToggle, pose, rate * dt);
            }
        }

        private abstract class KeyProcessor
        {
            protected const float CTrans = 1f;
            protected const float CRot = 0.25f;

            public abstract bool getNextTarget(ref ComData d, Collection<Key> keysDown, Collection<Key> keysToggle, ArmPose pose, float stepAmount);
        }

        private class PosDirProcessor : KeyProcessor
        {
            public override bool getNextTarget(ref ComData d, Collection<Key> keysDown, Collection<Key> keysToggle, ArmPose pose, float stepAmount)
            {
                Vector3 dPos = new Vector3();
                Vector3 dDir = new Vector3();   // just for storing dRotation angles
                if (keysDown.Contains(Key.A))
                    dPos.X += -1;
                if (keysDown.Contains(Key.D))
                    dPos.X += 1;
                if (keysDown.Contains(Key.S))
                    dPos.Y += -1;
                if (keysDown.Contains(Key.W))
                    dPos.Y += 1;
                if (keysDown.Contains(Key.C))
                    dPos.Z += -1;
                if (keysDown.Contains(Key.Z))
                    dPos.Z += 1;

                if (keysDown.Contains(Key.J))
                    dDir.Y += -1;
                if (keysDown.Contains(Key.L))
                    dDir.Y += 1;
                if (keysDown.Contains(Key.I))
                    dDir.X += -1;
                if (keysDown.Contains(Key.K))
                    dDir.X += 1;
                if (keysDown.Contains(Key.M))
                    dDir.Z += -1;
                if (keysDown.Contains(Key.OemPeriod))
                    dDir.Z += 1;

                if (dPos == Vector3.Zero && dDir == Vector3.Zero)
                    return false;

                dPos = dPos.LengthSquared() !=0? Vector3.Normalize(dPos) * stepAmount * CTrans : dPos;
                dDir = dDir.LengthSquared() !=0? Vector3.Normalize(dDir) * stepAmount * CRot : dDir;

                var dPose =  new ArmPose(dPos, dDir, dDir);
                d = new ComData(new KeyFrame(dPose, stepAmount));
                //Console.WriteLine(dDir.ToString());
                return true;
            }
        }

        private class RawAnglesProcessor : KeyProcessor
        {
            public override bool getNextTarget(ref ComData d, Collection<Key> keysDown, Collection<Key> keysToggle, ArmPose arm, float stepAmount)
            {
                stepAmount *= CRot;

                if (keysDown.Contains(Key.LeftShift) || keysDown.Contains(Key.RightShift))
                    stepAmount *= -1;

                Key[] toCheck = {
                    //Key.D0,
                    Key.D1, Key.D2,
                    Key.D3, Key.D4,
                    Key.D5, Key.D6 };
                Angle[] dAngles = new Angle[toCheck.Length];
                bool changed = false;
                for (int i = 0; i < toCheck.Length; i++)
                {
                    Key k = toCheck[i];
                    if(keysDown.Contains(k))
                    {
                        dAngles[i] = stepAmount;
                        changed = true;
                    }
                    else
                    {
                        dAngles[i] = 0;
                    }
                }

                //Console.WriteLine("raw");
                d = changed ? new ComData(dAngles) : d;
                return changed;
            }
        }
    }
}
