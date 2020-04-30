using System;

namespace AHdata
{
    public struct KeyFrame : IJSONable
    {
        public KeyFrame(KeyFrame d) : this(d.pose, d.dt)
        { }

        public KeyFrame(ArmPose p, float dt)
        {
            pose.position = p.position;
            pose.direction = p.direction;
            pose.handUp = p.handUp;

            this.dt = dt;
        }

        public ArmPose pose;
        public float dt;

        public void addToJSON(JSONBuilder jb)
        {
            jb.beginItem();

            jb.addField("dt");
            jb.addPrimitive(dt.ToString());

            jb.addField("pose");
            pose.addToJSON(jb);

            jb.endItem();
        }

        public void fillFromJSON(JSONDecoder jd)
        {
            jd.beginItem();

            dt = Single.Parse(jd.getField()[1]);    //dt

            jd.getField();                          //pose
            pose.fillFromJSON(jd);

            jd.endItem();
        }
    }
}
