using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace MersiveEpi
{
    public class MersiveConfig
    {
        public static MersiveConfig FromDeviceConfig(DeviceConfig config)
        {
            return JsonConvert.DeserializeObject<MersiveConfig>(config.Properties.ToString());
        }

        [JsonProperty("control")]
        public MersiveControlPropertiesConfig Control { get; set; }
        
        [JsonProperty("communicationMonitor")]
        public CommunicationMonitorConfig CommunicationMonitor { get; set; }

        public class MersiveControlPropertiesConfig
        {
            public string Method { get; set; }
            public MersiveControlPropertiesDetails TcpSshProperties { get; set; }
        }

        public class MersiveControlPropertiesDetails
        {
            public string Hostname { get; set; }
            public string Password { get; set; }
            public int Port { get; set; }
        }
    } 
}