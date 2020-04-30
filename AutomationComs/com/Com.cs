using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Collections;
using System.Diagnostics;

using AHdata;
using System.ComponentModel;
using System.IO;

namespace AHcoms
{
    public abstract class Com
    {
        private const bool VERBOSE = true;

        protected const int WAIT_Interval = 100;
        protected bool running = false;

        private readonly Thread readThread, writeThread;
        protected readonly AutoResetEvent readResetEvent, writeResetEvent;

        private LinkedList<Packet> sendQ;

        public delegate void onReadData(ComData data);
        private onReadData onReadDataDelegate = null;

        private BackgroundWorker connector;

        public Com()
        {
            readThread = new Thread(() => threadAbortContainer(readLoop));
            writeThread = new Thread(() => threadAbortContainer(writeLoop));

            sendQ = new LinkedList<Packet>();
            
            writeResetEvent = new AutoResetEvent(false);

            connector = new BackgroundWorker();
            connector.DoWork += connectProcedure;
            connector.RunWorkerCompleted += connectionCompleted;
            connector.RunWorkerAsync();
        }

        protected abstract void connectProcedure(object sender, DoWorkEventArgs args);
        private void connectionCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            connector.Dispose();
            connector = null;

            readThread.Start();
            writeThread.Start();
        }

        public void setOnReadDataDelegate(onReadData onReadDataDelegate)
        {
            this.onReadDataDelegate = onReadDataDelegate;
        }

        private Packet lastRequest = null;
        private void requestSend(Packet p)
        {
            //Console.WriteLine("----rsend: " + p.key.value + " " + p.flavour.ToString());
            if (lastRequest != null && lastRequest.key == p.key) return;
            lock (sendQ)
            {
                sendQ.AddLast(p);
                lastRequest = p;
            }

            if (p.flavour != Packet.Flavour.ECHO)
            {
                string snippet = p.data.dataType;
                if(VERBOSE) Console.WriteLine("Send: " + p.key.value + " " + snippet);
            }

            writeResetEvent.Set();
        }

        public void sendData(IJSONable data)
        {
            sendData(new ComData(data));
        }

        public void sendData(IJSONable[] arr)
        {
            sendData(new ComData(arr));
        }

        public void sendData(ComData d)
        {
            Packet p = new Packet(d);
            requestSend(p);
        }

        private void writeLoop()
        {
            long last = Stopwatch.GetTimestamp();
            while (true)
            {
                Packet msg = null;
                lock (sendQ)
                {
                    if (sendQ.Count > 0)
                    {
                        msg = sendQ.First();
                        sendQ.RemoveFirst();
                        //if (msg.flavour != Packet.Flavour.ECHO) sendQ.AddLast(msg);
                    }
                }

                if (msg != null)
                {
                    string toSend = msg.pack();
                    send(toSend);   // may block on this
                }
                else
                {
                    writeResetEvent.WaitOne();
                }
            }
        }

        protected abstract void send(string msg);

        private void threadAbortContainer(Action run)
        {
            try
            {
                run();
            }
            catch (ThreadAbortException)
            {
                Environment.Exit(0);
            }

        }

        private void readLoop()
        {
            long lastDataKey = 0;

            while (true)
            {
                string msg = read();
                //Packet p = readSer.Deserialize<Packet>(msg);
                Packet p;
                try
                {
                    p = Packet.unPack(msg);
                } catch (InvalidDataException)
                {
                    continue;
                } catch (NullReferenceException)
                {
                    continue;
                }

                if (p.flavour == Packet.Flavour.ECHO)
                {
                    lock(sendQ)
                    {
                        removePacket(p);
                    }
                    if (VERBOSE) Console.WriteLine("Echo>>" + p.key.value);
                }
                else
                {
                    ////send back echo
                    //Packet echo = Packet.echo(p);
                    //requestSend(echo);
                    if (onReadDataDelegate != null)
                    {
                        if(lastDataKey < p.key.value)
                        {
                            if(VERBOSE) Console.WriteLine("Read: " + p.key.value + " " + p.data.dataType);
                            onReadDataDelegate(p.data);
                            lastDataKey = p.key.value;
                        }
                    }
                }
            }
        }

        private void removePacket(Packet p)
        {
            LinkedListNode<Packet> curr = sendQ.First;
            while(!(curr is null))
            {
                Packet c = curr.Value;
                if(c.key == p.key)
                {
                    sendQ.Remove(curr);
                }
                curr = curr.Next;
            }
        }

        protected abstract string read();

        protected abstract void dispose();

        public void close()
        {
            lock (this)
            {
                running = false;
            }
            writeThread.Abort();
            readThread.Abort();

            try
            {
                Packet close = new Packet(new ComData(ComRequest.CLOSE));
                send(close.pack());

                writeThread.Join();
                readThread.Join();
            }catch(ThreadStateException) { }

            dispose();
        }
    }
}
