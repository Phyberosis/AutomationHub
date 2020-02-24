using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.ComponentModel;

using AutomationHub.controller;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace AutomationHub
{
    public abstract class Controller
    {
        protected Collection<Key> keysDown;
        protected Collection<Key> keysToggle;

        protected ICom com;
        protected Arm arm;

        private BackgroundWorker worker;

        private bool hasControl = false;

        public delegate void UpdateLabels(VerboseInfo msg);
        private UpdateLabels updateLabelsDelegate;

        public Controller(ICom com)
        {
            this.com = com;
            arm = ArmFactory.makeDefaultArm();

            worker = new BackgroundWorker();
            worker.DoWork += runControl;

            keysDown = new Collection<Key>();
            keysToggle = new Collection<Key>();
        }
        
        public ArmData getArmData()
        {
            return arm.getData();
        }

        public void GiveControl(UpdateLabels updateLabelsDelegate)
        {
            if (hasControl) return;

            this.updateLabelsDelegate = updateLabelsDelegate;
            arm.setServoAngles(com.getCurrentServoAngles());
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
            hasControl = true;
        }

        public void ReleaseControl()
        {
            if (!hasControl) return;

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

        private void runControl(object sender, DoWorkEventArgs e)
        {
            long last = currentTimeMilis();
            while (!worker.CancellationPending)
            {
                long now = currentTimeMilis();
                step((now - last) / 10000000f);

                VerboseInfo info;
                Vector3 dir = arm.getDirection();
                Vector3 pos = arm.getPosition();

                info.dir = "<" + dir.X.ToString("0.00") + ", " + dir.Y.ToString("0.00") + ", " + dir.Z.ToString("0.00") + ">";
                info.pos = "<" + pos.X.ToString("0.00") + ", " + pos.Y.ToString("0.00") + ", " + pos.Z.ToString("0.00") + ">";
                info.msg = "";
                foreach(Key k in keysDown)
                {
                    info.msg += k.ToString() + ".";
                }
                info.msg += "\n";
                foreach (Angle a in arm.getServoAngles())
                {
                    info.msg += a.get().ToString() + "\n";
                }
                updateLabelsDelegate(info);

                last = now;

                System.Threading.Thread.Sleep(15);
            }
        }

        protected abstract void step(float dt);
    }
}
