using System.Collections.Generic;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace MersiveEpi
{
    public class MersiveFactory : EssentialsPluginDeviceFactory<MersiveSolsticeDevice>
    {
        public MersiveFactory()
        {
            MinimumEssentialsFrameworkVersion = "1.8.0";
            TypeNames = new List<string>
                {
                    "MersiveSolstice"
                };
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            return new MersiveSolsticeDevice(dc);
        }
    }
}