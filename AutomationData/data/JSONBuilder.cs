using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHdata
{
    public class JSONBuilder
    {
        private StringBuilder sb;
        private int indents = 0;
        private static readonly string newLine = Environment.NewLine;//"^|v";

        public JSONBuilder()
        {
            sb = new StringBuilder();
        }

        public static string convertNewLines(string str)
        {
            return str.Replace(newLine, Environment.NewLine);
        }

        private void addIndents()
        {
            for(int i = 0; i<indents; i++)
            {
                sb.Append("  ");
            }
        }

        bool justInArr = false;
        public void beginArray()
        {
            sb.Append(newLine);
            addIndents();
            sb.Append("[");
            indents++;

            inField = false;
            justInArr = true;
        }

        public void endArray()
        {
            endItem("]");
        }

        public void beginItem()
        {
            sb.Append(newLine);
            addIndents();
            sb.Append("{" + newLine);
            indents++;

            inField = false;
            justInArr = false;
        }

        //private bool cascadeEnd = false;
        public void endItem(string cb = "}")
        {
            indents--;
            addIndents();
            sb.Append(cb);
            sb.Append(newLine);
            //cascadeEnd = true;
        }

        private bool inField = false;
        public void addField(string name)
        {
            //sb.Append(newLine);
            addIndents();
            sb.Append(name + ": ");
            inField = true;
        }

        public void addPrimitive(string val)
        {
            if(justInArr)
            {
                sb.Append(newLine);
                justInArr = false;
            }

            if(!inField) addIndents();
            sb.Append(val + newLine);
            inField = false;
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
