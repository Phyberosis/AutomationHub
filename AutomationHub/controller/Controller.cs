using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;

using AHdata;
using AHcoms;
using System.Threading;
using System.ComponentModel;

namespace AutomationHub
{
    public abstract class Controller
    {
        protected Collection<Key> keysDown;
        protected Collection<Key> keysToggle;

        protected Com comObject;
        protected ArmPose pose;
        protected Angle[] servoAngles;

        private BackgroundWorker worker;
        private readonly AutoResetEvent workerResumeEvent;

        private bool hasControl = false;

        public delegate void UpdateLabels(VerboseInfo msg);
        private UpdateLabels updateLabelsDelegate;
        private VerboseInfo infoForLabels;

        public Controller(Com comObject)
        {
            this.comObject = comObject;

            worker = new BackgroundWorker();
            worker.DoWork += runControl;
            workerResumeEvent = new AutoResetEvent(false);

            keysDown = new Collection<Key>();
            keysToggle = new Collection<Key>();
        }
        
        //public KeyFrame getArmData()
        //{
        //    return pose.getData();
        //}

        public void GiveControl(UpdateLabels updateLabelsDelegate)
        {
            if (hasControl) return;

            this.updateLabelsDelegate = updateLabelsDelegate;
            //arm.setServoAngles(comObject.getCurrentServoAngles());
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
            hasControl = true;
        }

        public void ReleaseControl()
        {
            if (!hasControl) return;

            workerResumeEvent.Set();
            worker.CancelAsync();
            hasControl = false;
        }

        public bool inControl()
        {
            return hasControl;
        }

        private long currentTimeMilis()
        {
            return DateTime.Now.Ticks;
        }

        public void keyDown(object sender, KeyEventArgs e)
        {
            lock(this)
            {
                if (!keysDown.Contains(e.Key)) keysDown.Add(e.Key);
                if (!keysToggle.Contains(e.Key))
                {
                    keysToggle.Add(e.Key);
                }
                else
                {
                    keysToggle.Remove(e.Key);
                }
            }
        }

        public void keyUp(object sender, KeyEventArgs e)
        {
            lock (this)
            {
                if (keysDown.Contains(e.Key)) keysDown.Remove(e.Key);
            }
        }

        private void waitForInitInfo()
        {
            VerboseInfo i = new VerboseInfo();
            i.msg = "waiting on initial pose...";
            updateLabelsDelegate(i);

            comObject.setOnReadDataDelegate(updateRecieved);
            comObject.sendData(ComRequest.POSE);
            //comObject.sendData(ComRequest.ANGLES);

            //workerResumeEvent.WaitOne();    //psoe
            workerResumeEvent.WaitOne();    //angles
        }

        private void updateRecieved(ComData data)
        {
            if (data.isType<ArmPose>())
            {
                pose = (ArmPose)data.value[0];
                updatePoseInfo();
                workerResumeEvent.Set();
            }
            else if (data.isType<Angle>())
            {
                servoAngles = new Angle[data.value.Length];
                for (int i = 0; i<data.value.Length; i++)
                {
                    servoAngles[i] = (Angle)data.value[i];
                }
                updateAngleInfo();
            }

            updateLabelsDelegate(infoForLabels);
        }

        private void runControl(object sender, DoWorkEventArgs e)
        {
            waitForInitInfo();  // blocking

            long last = currentTimeMilis();
            long lastAngleUpdate = currentTimeMilis();
            while (!worker.CancellationPending)
            {
                long now = currentTimeMilis();
                float dt = (now - last) / 10000000f;
                if (dt < 0.02) continue;

                ComData target = new ComData();
                if (step(ref target, dt))
                {
                    comObject.sendData(target);
                }

                if ((now - lastAngleUpdate) / 10000000f >= 1f)
                {
                    comObject.sendData(ComRequest.ANGLES);
                    comObject.sendData(ComRequest.POSE);
                    lastAngleUpdate = now;
                }

                updatePoseInfo();
                updateAngleInfo();
                infoForLabels.msg = "";

                lock(this)
                {
                    foreach (Key k in keysDown)
                    {
                        infoForLabels.msg += k.ToString() + ".";
                    }
                }
                updateLabelsDelegate(infoForLabels);

                last = now;
            }
        }

        protected void updatePoseInfo()
        {
            ArmPose p = pose;
            Vector3 dir = p.direction;
            Vector3 pos = p.position;
            Vector3 hUp = p.handUp;

            infoForLabels.dir = "<" + dir.X.ToString("0.00") + ", " + dir.Y.ToString("0.00") + ", " + dir.Z.ToString("0.00") + ">";
            infoForLabels.pos = "<" + pos.X.ToString("0.00") + ", " + pos.Y.ToString("0.00") + ", " + pos.Z.ToString("0.00") + ">";
            infoForLabels.hUp = "<" + hUp.X.ToString("0.00") + ", " + hUp.Y.ToString("0.00") + ", " + hUp.Z.ToString("0.00") + ">";
        }

        protected void updateAngleInfo()
        {
            if (servoAngles == null) return;
            Angle[] angles = servoAngles;
            infoForLabels.angles = "";
            for (int i = 0; i < angles.Length; i++)
            {
                servoAngles[i] = angles[i];
                infoForLabels.angles += servoAngles[i].get() + "\n";
            }
        }

        protected abstract bool step(ref ComData d, float dt);
    }
}
