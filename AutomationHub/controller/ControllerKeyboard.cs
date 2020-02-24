using AutomationHub.arm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutomationHub.controller
{
    internal class ControllerKeyboard : Controller
    {
        private KeyProcessor kpPosDir, kpRaw;
        public ControllerKeyboard(ICom com) : base(com)
        {
            kpPosDir = new PosDirProcessor();
            kpRaw = new RawAnglesProcessor();
        }

        protected override void step(float dt)
        {
            KeyProcessor keyProcessor = keysToggle.Contains(Key.CapsLock)? kpRaw : kpPosDir;

            float rate = keysToggle.Contains(Key.F)? 10 : 2.5f;

            lock (this)
            {
                keyProcessor.updateArm(keysDown, keysToggle, arm, rate * dt);
            }
            com.flushPose(arm.getData());
        }

        private abstract class KeyProcessor
        {
            protected const float CTrans = 1f;
            protected const float CRot = 0.25f;

            public abstract void updateArm(Collection<Key> keysDown, Collection<Key> keysToggle, Arm arm, float dt);
        }

        private class PosDirProcessor : KeyProcessor
        {
            public override void updateArm(Collection<Key> keysDown, Collection<Key> keysToggle, Arm arm, float stepAmount)
            {
                Vector3 dPos = new Vector3();
                Vector3 dDir = new Vector3();   // axis of rot
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

                dPos = dPos.LengthSquared() !=0? Vector3.Normalize(dPos) * stepAmount * CTrans : dPos;
                Vector3 pos = Vector3.Add(arm.getPosition(), dPos);

                dDir = dDir.LengthSquared() !=0? Vector3.Normalize(dDir) * stepAmount * CRot : dDir;
                Matrix4x4 rotX = Matrix4x4.CreateRotationX(dDir.X);
                Matrix4x4 rotY = Matrix4x4.CreateRotationY(dDir.Y);
                Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(dDir.Z);
                Vector3 dir = arm.getDirection();
                dir = Vector3.Transform(dir, rotX);
                dir = Vector3.Transform(dir, rotY);
                dir = Vector3.Transform(dir, rotZ);

                arm.setPose(pos, dir);
                //Console.WriteLine(dDir.ToString());
            }
        }

        private class RawAnglesProcessor : KeyProcessor
        {
            public override void updateArm(Collection<Key> keysDown, Collection<Key> keysToggle, Arm arm, float stepAmount)
            {
                stepAmount *= CRot;

                if (keysDown.Contains(Key.LeftShift) || keysDown.Contains(Key.RightShift))
                    stepAmount *= -1;

                Angle[] curAngles = arm.getServoAngles();
                Angle[] nextAngles = new Angle[curAngles.Length];
                Key[] toCheck = {
                    Key.D0,
                    Key.D1, Key.D2,
                    Key.D3, Key.D4,
                    Key.D5, Key.D6 };
                for (int i = 0; i < toCheck.Length; i++)
                {
                    Key k = toCheck[i];
                    nextAngles[i] = keysDown.Contains(k) ? curAngles[i] + stepAmount : curAngles[i];
                }

                //Console.WriteLine("raw");
                arm.setServoAngles(nextAngles);
            }
        }
    }
}
