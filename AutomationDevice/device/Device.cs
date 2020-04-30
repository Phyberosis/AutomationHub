using AHcoms;
using AHdata;

namespace AutomationDevice
{
    public abstract class Device
    {
        protected Com com;

        public Device()
        {
            com = ComFactory.getDefaultCom();
            com.setOnReadDataDelegate(onDataRecieved);
        }

        protected abstract void onDataRecieved(ComData data);
    }
}
