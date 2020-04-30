using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AHdata
{
    public struct ArmPose : IJSONable
    {
        //public ArmPose()
        //{
        //    position = Vector3.Zero;
        //    direction = Vector3.UnitZ;
        //    handUp = -Vector3.UnitX;
        //}

        public ArmPose(ArmPose p)
        {
            position = p.position;
            direction = p.direction;
            handUp = p.handUp;
        }

        public ArmPose(Vector3 p, Vector3 d, Vector3 u)
        {
            position = p;
            direction = d;
            handUp = u;
        }

        public Vector3 position;
        public Vector3 direction;
        public Vector3 handUp;

        public void addToJSON(JSONBuilder jb)
        {
            jb.beginItem();
            jb.addField("position");
            jb.addPrimitive(position.ToString());
            jb.addField("direction");
            jb.addPrimitive(direction.ToString());
            jb.addField("handUp");
            jb.addPrimitive(handUp.ToString());
            jb.endItem();
        }

        public void fillFromJSON(JSONDecoder jd)
        {
            jd.beginItem();

            position = parseVec(jd.getField()[1]);
            direction = parseVec(jd.getField()[1]);
            handUp= parseVec(jd.getField()[1]);

            jd.endItem();
        }

        private Vector3 parseVec(string vec)
        {
            vec = vec.Substring(1, vec.Length - 2);
            char[] sep = { ',' };
            string[] vals = vec.Split(sep);

            Vector3 v;
            v.X = float.Parse(vals[0].Trim());
            v.Y = float.Parse(vals[1].Trim());
            v.Z = float.Parse(vals[2].Trim());

            return v;
        }
    }
}
