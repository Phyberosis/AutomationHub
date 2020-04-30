using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AHdata;

namespace AHcoms
{

    public class PacketKey : IJSONable
    {
        public long value;

        public PacketKey(PacketKey other)
        {
            value = other.value;
        }

        public PacketKey()
        {
            value = Stopwatch.GetTimestamp();
        }

        //public static PacketKey responseKey(PacketKey original)
        //{
        //    var p = new PacketKey();
        //    p.parentValue = original.value;
        //    return p;
        //}

        //public bool isResponseOf(PacketKey other)
        //{
        //    return parentValue == other.value;
        //}

        public void addToJSON(JSONBuilder jb)
        {
            jb.beginItem();

            jb.addField("value");
            jb.addPrimitive(value.ToString());

            jb.endItem();
        }

        public void fillFromJSON(JSONDecoder jd)
        {
            jd.beginItem();
            value = long.Parse(jd.getField()[1]);
            jd.endItem();
        }

        public static bool operator <=(PacketKey me, PacketKey other)
        {
            if (me is null || other is null) return false;
            return me.value <= other.value;

        }

        public static bool operator >=(PacketKey me, PacketKey other)
        {
            if (me is null || other is null) return false;
            return me.value >= other.value;

        }

        public static bool operator ==(PacketKey me, PacketKey other)
        {
            if (me is null || other is null) return false;
            return me.value.Equals(other.value);
        }

        public static bool operator !=(PacketKey me, PacketKey other)
        {
            if (me is null || other is null) return true;
            return !me.value.Equals(other.value);
        }
    }

    public class Packet
    {
        public enum Flavour
        {
            DATA, ECHO
        }

        public readonly Flavour flavour;
        public readonly PacketKey key;

        public readonly ComData data;

        private Packet(Packet p)
        {
            flavour = p.flavour;
            key = p.key;
            data = p.data;
        }

        public Packet(ComData d)
        {
            data = d;
            flavour = Flavour.DATA;
            key = new PacketKey();
        }

        public Packet(Flavour f)
        {
            flavour = f;
            key = new PacketKey();
        }

        private Packet(Flavour f, PacketKey k)
        {
            flavour = f;
            key = new PacketKey(k);
        }

        private Packet(Flavour f, PacketKey k, ComData d) : this(f, k)
        {
            data = d;
        }

        public static Packet echo(Packet p)
        {
            return new Packet(Flavour.ECHO, p.key);
        }

        public string pack()
        {
            JSONBuilder jb = new JSONBuilder();
            jb.beginItem();

            jb.addField("flavour");
            jb.addPrimitive(flavour.ToString());

            jb.addField("key");
            key.addToJSON(jb);

            if(flavour != Flavour.ECHO)
            {
                jb.addField("data");
                data.addToJSON(jb);
            }

            jb.endItem();

            return jb.ToString();
        }

        public static Packet unPack(string s)
        {
            int a = s.Length - s.Replace("{", "").Replace("[", "").Length;
            int b = s.Length - s.Replace("}", "").Replace("]", "").Length;
            string checkS = s.Replace(Environment.NewLine, "").Trim();
            if (a != b || !checkS.Substring(0, 1).Equals("{") || !checkS.Substring(checkS.Length - 3, 1).Equals("}") ||
                !checkS.Contains("flavour") || !checkS.Contains("key"))
            {
                Console.WriteLine("Packet: invalid string");
                Console.WriteLine(s);
                Console.WriteLine("-- end print --");
                Console.WriteLine(checkS.Substring(checkS.Length - 2, 1).ToCharArray()[0]);
                //Console.WriteLine(checkS.Substring(checkS.Length - 3, 1));
                //Console.WriteLine("a: {0}, b: {1}", a, b);
                throw new InvalidDataException();
            }

            Flavour f;
            PacketKey k = new PacketKey();
            ComData d = new ComData();

            JSONDecoder jd = new JSONDecoder(s);
            jd.beginItem();

            string flav = jd.getField()[1].Trim();
            f = (Flavour)Enum.Parse(typeof(Flavour), flav);

            jd.getField();  // key
            k.fillFromJSON(jd);

            if (f != Flavour.ECHO)
            {
                jd.getField();  // data
                d.fillFromJSON(jd);
            }

            jd.endItem();

            return new Packet(f, k, d);
        }

        public override bool Equals(object obj)
        {
            if(obj is Packet)
            {
                Packet other = (Packet)obj;
                return flavour == other.flavour && key == other.key;
            }

            return false;
        }

        public bool isEchoOf(Packet other)
        {
            return key == other.key;
        }
    }
}
