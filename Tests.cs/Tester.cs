using AHdata;
using AHcoms;

using System;
using System.Numerics;
using System.IO;
using System.Text;

namespace Tests.cs
{
    class Tester
    {
        static void Main(string[] args)
        {
            Tester p = new Tester();
            //p.runTests();

            //ComLANexp a = new ComLANexp();
            //a.run();

            Console.Read();
        }

        void runTests()
        {
            testPose();
            testRequest();
            testKeyFrame();
            testAngleArr();
            testEcho();
            testBad();
        }

        void printName(string name)
        {
            Console.WriteLine("### " + name + " ###\n\n");
        }

        bool testJSON(IJSONable obj)
        {
            return testHelper(new Packet(new ComData(obj)));
        }

        bool testJSON(IJSONable[] arr)
        {
            return testHelper(new Packet(new ComData(arr)));
        }

        bool testHelper(Packet pp)
        {
            string first = pp.pack();
            Console.WriteLine(first);
            Console.WriteLine();

            Packet back = Packet.unPack(first);
            string second = back.pack();

            Console.WriteLine(second);

            bool pass = first.Equals(second);

            if(!pass)
            {
                Console.WriteLine("@@@@@@@@ Fail @@@@@@@@");
            }
            return pass;
        }

        void testPose()
        {
            printName("Pose");

            ArmPose p = new ArmPose();
            p.position = Vector3.UnitX;
            p.direction = Vector3.UnitY;
            p.handUp = Vector3.UnitZ;

            testJSON(p);
        }

        void testAngleArr()
        {
            printName("AngleArr");

            Angle[] a =
{
                new Angle(1),
                new Angle(2),
                new Angle(3),
                new Angle(4)
            };

            testJSON(a);
        }

        void testRequest()
        {
            printName("Request");
            
            testJSON(ComRequest.POSE);
        }

        void testKeyFrame()
        {
            printName("KeyFrame");

            ArmPose p = new ArmPose();
            p.position = Vector3.UnitX;
            p.direction = Vector3.UnitY;
            p.handUp = Vector3.UnitZ;
            KeyFrame k = new KeyFrame(p, 0.444f);

            testJSON(k);
        }

        void testEcho()
        {
            printName("Echo");
            Packet o = new Packet(new ComData(ComRequest.POSE));
            Console.WriteLine("Original key: " + o.key.value);
            testHelper(Packet.echo(o));
        }

        void testBad()
        {
            printName("test bad input");

            string test = File.ReadAllText(@"C:\workspace\CSharp\AutomationHub\Tests.cs\bad.txt");

            try
            {
                JSONDecoder jd = new JSONDecoder(test);
            }catch(InvalidDataException)
            {
                return; //pass
            }

            Console.WriteLine("FAIL");
        }
    }
}
