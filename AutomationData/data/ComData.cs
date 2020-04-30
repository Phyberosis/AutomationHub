using System;
using System.Collections.Generic;

namespace AHdata
{
    public class ComRequest : IJSONable
    {
        private enum Request
        {
            POSE, ANGLES, CLOSE
        }

        public static readonly ComRequest POSE = new ComRequest(Request.POSE);
        public static readonly ComRequest ANGLES = new ComRequest(Request.ANGLES);
        public static readonly ComRequest CLOSE = new ComRequest(Request.CLOSE);

        private Request request;

        public ComRequest()
        {

        }

        private ComRequest(Request r)
        {
            request = r;
        }

        public void addToJSON(JSONBuilder jb)
        {
            jb.addPrimitive(request.ToString());
        }

        public void fillFromJSON(JSONDecoder jd)
        {
            request = (Request)Enum.Parse(typeof(Request), jd.getPrimitive()[1]);
        }

        public static bool isSame(ComRequest a, ComRequest b)
        {
            return a.request.Equals(b.request);
        }
    }

    public class ComData : IJSONable
    {
        public IJSONable[] value;
        public string dataType;

        public ComData()
        { }

        public ComData(ComRequest c)
        {
            dataType = typeof(ComRequest).ToString();
            value = new ComRequest[]{ c };
        }

        public ComData(IJSONable o)
        {
            value = new IJSONable[] { o };
            dataType = o.GetType().ToString();
        }

        public ComData(IJSONable[] arr)
        {
            value = arr;
            dataType = arr[0].GetType().ToString();
        }

        public bool isType<T>()
        {
            return dataType.Equals(typeof(T).ToString());
        }

        public void addToJSON(JSONBuilder jb)
        {
            jb.beginItem();

            jb.addField("dataType");
            jb.addPrimitive(dataType);

            jb.addField("value");
            jb.beginArray();
            foreach(IJSONable obj in value)
            {
                obj.addToJSON(jb);
            }
            jb.endArray();

            jb.endItem();
        }

        public void fillFromJSON(JSONDecoder jd)
        {
            jd.beginItem();

            dataType = jd.getField()[1];    // dataType

            jd.getField();                  // data
            jd.beginItem();                 // [
            Type t = Type.GetType(dataType);
            Queue<IJSONable> dataQ = new Queue<IJSONable>();
            while(!jd.peekLine().Contains("]"))
            {
                IJSONable obj = (IJSONable)Activator.CreateInstance(t);
                obj.fillFromJSON(jd);
                dataQ.Enqueue(obj);
            }
            value = dataQ.ToArray();
            jd.endItem();

            jd.endItem();
        }
    }
}
