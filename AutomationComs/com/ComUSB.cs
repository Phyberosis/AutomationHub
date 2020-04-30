using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AHdata;

namespace AHcoms
{
    internal class ComUSB : Com
    {
        private const bool ENABLE = false;
        //private SerialPort mSerialPort;

        private const String mPortName = "COM5";
        private const int mBaudRate = 57600;
        private const Parity mParity = Parity.None;
        private const int mDataBits = 8;
        private const StopBits mStopBits = StopBits.One;

        public ComUSB() : base()
        {
            throw new NotImplementedException();
            //mSerialPort.ReadTimeout = WAIT_Interval;
        }

        public void flushPose(KeyFrame data)
        {
            //Console.WriteLine("comUSB flush");
        }

        protected override void send(string msg)
        {
            Console.WriteLine(msg);
        }

        public bool confirmed()
        {
            return true;
        }

        protected override string read()
        {
            throw new NotImplementedException();
        }

        protected override void connectProcedure(object sender, DoWorkEventArgs args)
        {
            throw new NotImplementedException();
        }

        protected override void dispose()
        {
            throw new NotImplementedException();
        }

        //public Angle[] getCurrentServoAngles()
        //{
        //    Angle[] temp = new Angle[6];
        //    for(int i =0; i<7; i++)
        //    {
        //        temp[i] = new Angle();
        //    }
        //    return temp;
        //}
    }
}
