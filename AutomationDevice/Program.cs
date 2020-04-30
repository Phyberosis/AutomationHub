using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace AutomationDevice
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Device d = DeviceFactory.getDefaultDevice();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            while(true)
            {
                Console.Read();
            }
        }
    }
}
