using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AHcoms
{
    public struct LANSettings
    {
        private static readonly string IP_STRING_HOST = "192.168.1.68";
        private static readonly string IP_STRING_CLIENT = "192.168.1.69";
        private static readonly int PORT = 8080;
        public static readonly IPEndPoint IP_HOST = new IPEndPoint(IPAddress.Parse(IP_STRING_HOST), PORT);
        public static readonly IPEndPoint IP_CLIENT = new IPEndPoint(IPAddress.Parse(IP_STRING_CLIENT), PORT);
    }

    internal class ComLAN : Com
    {
        private Socket connection;
        private readonly byte[] FLANK = new byte[]
        {
            0, 2, 4, 8, 16, 32, 64, 128
        };
        private readonly int CHUNK_SIZE;

        private readonly string[] acceptedSocketExceptions = {
            "An established connection was aborted by the software in your host machine",
            "An existing connection was forcibly closed by the remote host"
        };

        public ComLAN() : base()
        {
            CHUNK_SIZE = FLANK.Length;
        }

        protected override void connectProcedure(object sender, DoWorkEventArgs args)
        {
            IPEndPoint[] IPs = new IPEndPoint[]
            {
                LANSettings.IP_HOST,
                LANSettings.IP_CLIENT
            };

            string[] connectErrors = new string[]
            {
                "Only one usage of each socket address (protocol/network address/port) is normally permitted",
                "The requested address is not valid in this context"
            };

            foreach(IPEndPoint ip in IPs)
            {
                Console.WriteLine("Init at " + ip.ToString());
                Socket server = new Socket(AddressFamily.InterNetwork,
                                SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Bind(ip);
                    server.Listen(3);
                    Console.WriteLine("Started as server, waiting for device client");

                    connection = server.Accept();
                    Console.WriteLine("Client connected");
                }
                catch (SocketException e)
                {
                    if (e.Message.Contains(connectErrors[0]) || e.Message.Contains(connectErrors[1]))
                    {
                        Console.WriteLine("Port occupied, started as client");
                        server.Connect(ip);
                        Console.WriteLine("Connected to server");

                        connection = server;
                    }
                    else
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                }
                break;
            }
        }

        protected override string read()
        {
            try
            {
                return readOp();
            }
            catch (SocketException e)
            {
                handleSocketExceptions(e);
                return "";
            }
        }

        protected override void send(string msg)
        {
            try
            {
                sendOp(msg);
            }
            catch (SocketException e)
            {
                handleSocketExceptions(e);
            }

        }
        private void handleSocketExceptions(SocketException e)
        {
            foreach(string m in acceptedSocketExceptions)
            {
                if (e.Message.Contains(m))
                {
                    Console.WriteLine("Connection terminated");
                    close();

                    return;
                }
            }

            throw e;
        }

        private string readOp()
        {
            int fLoc = 0;
            bool inBody = false;
            LinkedList<char> msg = new LinkedList<char>();
            bool wasInBody = false;

            while(!wasInBody || inBody)
            {
                byte[] chunk = new byte[CHUNK_SIZE];
                connection.Receive(chunk);

                wasInBody = wasInBody || inBody;

                foreach (byte b in chunk)
                {
                    if (inBody)
                    {
                        msg.AddLast(Convert.ToChar(b));

                        fLoc = b == FLANK[fLoc - 1] ? fLoc - 1 : CHUNK_SIZE;
                        inBody = fLoc != 0;
                    }
                    else
                    {
                        fLoc = b == FLANK[fLoc] ? fLoc + 1 : 0;
                        inBody = fLoc == CHUNK_SIZE;
                    }
                }
            }

            for(int i=0; i<CHUNK_SIZE; i++) msg.RemoveLast();
            string ret = new string(msg.ToArray()).Trim();
            return ret;
        }

        private void sendOp(string msg)
        {
            byte[] f = new byte[CHUNK_SIZE];
            Array.Copy(FLANK, f, CHUNK_SIZE);
            connection.Send(f);

            char[] rawM = msg.ToCharArray();
            int i = 0;
            int chunks = rawM.Length / CHUNK_SIZE;
            byte[] chunk = new byte[CHUNK_SIZE];
            for(int ch = 0; ch < chunks; ch++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    chunk[j] = Convert.ToByte(rawM[i]);
                    i++;
                }
                connection.Send(chunk);
            }
            for (int j = 0; j < CHUNK_SIZE; j++)
            {
                if(i < rawM.Length)
                {
                    chunk[j] = Convert.ToByte(rawM[i]);
                    i++;
                }
                else
                {
                    chunk[j] = Convert.ToByte(' ');
                    i++;
                }
            }
            connection.Send(chunk);

            Array.Reverse(f);
            connection.Send(f);
        }

        protected override void dispose()
        {
            connection.Close();
        }
    }
}
