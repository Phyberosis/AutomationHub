using AutomationDevice.devices;

namespace AutomationDevice
{
    public class DeviceFactory
    {
        public static Device getDefaultDevice()
        {
            return new Arm6Axis();
        }
    }
}
