using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.cs
{
    class ComLANexp
    {
        byte[] head = new byte[]
        {
            0, 2, 4, 8, 16, 32, 64, 128
        };

        byte[] tail = new byte[]
        {
            255, 127, 63, 31, 15, 7, 3, 1
        };

        //public void run()
        //{
        //    string testA = File.ReadAllText(@"C:\workspace\CSharp\AutomationHub\Tests.cs\bad.txt");
        //    string testB = "{" + Environment.NewLine + "}";
        //    const int size = 8;

        //    Queue<Byte> buffer = new Queue<byte>();



        //    byte[] msg = Encoding.UTF8.GetBytes(testA);

        //    foreach(byte b in head)
        //    {
        //        buffer.Enqueue(b);
        //    }
        //    foreach (byte b in msg)
        //    {
        //        buffer.Enqueue(b);
        //    }
        //    foreach (byte b in tail)
        //    {
        //        buffer.Enqueue(b);
        //    }

        //    //=====================================
            
        //    string hh = Encoding.UTF8.GetString(head);
        //    string tt = Encoding.UTF8.GetString(tail);
        //    //Console.WriteLine(hh);
        //    //Console.WriteLine(tt);
        //    Console.WriteLine(o);
        //}

        //private string read(Queue<byte> buffer)
        //{
            
        //}
    }
}
