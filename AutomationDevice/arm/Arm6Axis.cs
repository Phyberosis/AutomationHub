using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using WiringPi;
using static WiringPi.GPIO;

using AHdata;

namespace AutomationDevice.devices
{
    internal class Arm6Axis : Device
    {
        private ArmPose pose;
        private Angle[] targetAngles;
        private Angle[] servoAngles;

        private long lastRecieved;

        private Thread workThread;

        public Arm6Axis()
        {
            pose = new ArmPose();
            pose.direction = Vector3.UnitX;
            pose.handUp = Vector3.UnitZ;
            targetAngles = new Angle[6];
            servoAngles = new Angle[6];
            for (int i = 0; i< targetAngles.Length; i++)
            {
                targetAngles[i] = new Angle();
                servoAngles[i] = new Angle();
            }

            workThread = new Thread(stepperLoop);
            workThread.Start();

            Thread.Sleep(Timeout.Infinite);
        }

        private void stepperLoop()
        {
            try
            {
                if (Init.WiringPiSetup() == -1) Console.WriteLine("WiPi init failed");
                if (Init.WiringPiSetupGpio() == -1) Console.WriteLine("GPIO init failed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            const int HIGH = (int)GPIOpinvalue.High;
            const int LOW = (int)GPIOpinvalue.Low;

            GPIO.pinMode(0, (int)GPIOpinmode.Output);
            GPIO.pinMode(1, (int)GPIOpinmode.Output);
            GPIO.pinMode(2, (int)GPIOpinmode.Output);
            GPIO.digitalWrite(1, LOW);
            GPIO.digitalWrite(2, HIGH);

            Console.WriteLine("gpio started");

            while (true)
            {
                GPIO.digitalWrite(0, HIGH);
                Thread.Sleep(5);
                GPIO.digitalWrite(0, LOW);
            }
            
            Thread.Sleep(Timeout.Infinite);
        }

        protected override void onDataRecieved(ComData data)
        {
            if (data.isType<ComRequest>())
            {
                ComRequest req = (ComRequest)data.value[0];
                if (ComRequest.isSame(req, ComRequest.POSE))
                {
                    com.sendData(pose);
                }
                else if (ComRequest.isSame(req, ComRequest.CLOSE))
                {
                    Environment.Exit(0);
                }
                else if (ComRequest.isSame(req, ComRequest.ANGLES))
                {
                    com.sendData(targetAngles);
                }

                return;
            }
            else if (data.isType<KeyFrame>())
            {
                KeyFrame k = (KeyFrame)data.value[0];
                acceptKeyFrame(k);
            }
            else if (data.isType<Angle>())
            {
                for(int i =0; i<data.value.Length; i++)
                {
                    Angle a = (Angle)data.value[i];
                    targetAngles[i] += a;
                }
             }
            else
            {
                Console.WriteLine(data.dataType);
                Console.WriteLine("unk >> " + data.dataType + " : " + data.value.ToString());
            }

            updateController();
        }

        private void acceptKeyFrame(KeyFrame k)
        {
            Vector3 dPos = k.pose.position;
            Vector3 dDir = k.pose.direction;

            Vector3 pos = Vector3.Add(pose.position, dPos);
            Matrix4x4 rotX = Matrix4x4.CreateRotationX(dDir.X);
            Matrix4x4 rotY = Matrix4x4.CreateRotationY(dDir.Y);
            Matrix4x4 rotZ = Matrix4x4.CreateRotationZ(dDir.Z);

            Vector3 dir = pose.direction;
            //Console.WriteLine(dir.ToString());
            dir = Vector3.Transform(dir, rotX);
            dir = Vector3.Transform(dir, rotY);
            dir = Vector3.Transform(dir, rotZ);

            Vector3 up = pose.handUp;
            up = Vector3.Transform(up, rotX);
            up = Vector3.Transform(up, rotY);
            up = Vector3.Transform(up, rotZ);

            pose.position = pos;
            pose.direction = dir;
            pose.handUp = up;

            //Console.WriteLine(dir.ToString() + " " + Vector3.Transform(Vector3.UnitZ, rotX).ToString());
        }

        private void updateController()
        {
            long now = Stopwatch.GetTimestamp();
            float dt = (float)(now - lastRecieved) / (float)Stopwatch.Frequency;
            if (dt > 0.02)
            {
                com.sendData(pose);
                com.sendData(targetAngles);
                lastRecieved = now;
            }
        }
    }
}