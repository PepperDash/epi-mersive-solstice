using System;
using Newtonsoft.Json;

namespace MersiveEpi
{
    public class MersiveStatsResponse
    {
        [JsonProperty("m_displayId")]
        public string DisplayId { get; set; }

        [JsonProperty("m_displayInformation")]
        public MersiveDisplayInformation DisplayInformation { get; set; }

        [JsonProperty("m_serverVersion")]
        public string ServerVersion { get; set; }

        [JsonProperty("m_statistics")]
        public MersiveStatistics Statistics { get; set; }
    }

    public class MersiveDisplayInformation
    {
        [JsonProperty("m_displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("m_productHardwareVersion")]
        public long ProductHardwareVersion { get; set; }

        [JsonProperty("m_productName")]
        public string ProductName { get; set; }

        [JsonProperty("m_productVariant")]
        public string ProductVariant { get; set; }
    }

    public class MersiveStatistics
    {
        [JsonProperty("m_connectedUsers")]
        public long ConnectedUsers { get; set; }

        [JsonProperty("m_currentBandwidth")]
        public long CurrentBandwidth { get; set; }

        [JsonProperty("m_currentLiveSourceCount")]
        public long CurrentLiveSourceCount { get; set; }

        [JsonProperty("m_currentPostCount")]
        public long CurrentPostCount { get; set; }

        [JsonProperty("m_timeSinceLastConnectionInitialize")]
        public long TimeSinceLastConnectionInitialize { get; set; }
    }
}