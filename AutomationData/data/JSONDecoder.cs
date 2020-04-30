using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHdata
{
    public class JSONDecoder
    {
        private StringReader sr;

        public JSONDecoder(string s)
        {
            //Console.WriteLine(s);
            sr = new StringReader(s);
            //sr.ReadLine();
        }

        bool inArr = false;
        public void beginItem()
        {
            string s = peeked? currLine : sr.ReadLine();
            peeked = false;
            if (s.Contains("[")) inArr = true;
            //Console.WriteLine(s);
        }

        private string[] last;
        public string[] getPrimitive()
        {
            if (inArr)
            {
                string r = peeked? currLine : sr.ReadLine().Trim();
                peeked = false;
                return new string[] { "",  r};
            }
            return last;
        }

        private string currLine;
        private bool peeked = false;
        public string peekLine()
        {
            if (peeked) return currLine;

            peeked = true;
            currLine = sr.ReadLine();
            return currLine;
        }

        public string[] getField()
        {
            try
            {
                string line = peeked ? currLine : sr.ReadLine();
                peeked = false;
                //Console.WriteLine(line);
                int end = line.IndexOf(": ", 0);
                string name = line.Substring(0, end);
                string prim = line.Substring(end + 2, line.Length - end - 2);

                string[] ret = { name.Trim(), prim.Trim() };
                last = ret;
                return ret;
            }catch(Exception)
            {
                throw new InvalidDataException();
            }
        }

        public void endItem()
        {
            string s = peeked? currLine : sr.ReadLine();
            if (s.Contains("]")) inArr = false;
            peeked = false;
            //Console.WriteLine(s);
        }
    }
}
