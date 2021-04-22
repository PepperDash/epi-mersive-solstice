using PepperDash.Essentials.Core;

namespace MersiveEpi
{
    public class MersiveJoinMap : JoinMapBaseAdvanced
    {
        [JoinName("Id")] 
        public JoinDataComplete Id = new JoinDataComplete(
            new JoinData
                {
                    AttributeName = "Id",
                    JoinNumber = 2,
                    JoinSpan = 1
                },
            new JoinMetadata
                {
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial,
                    Description = "Id"
                });

        [JoinName("Is Online")] 
        public JoinDataComplete IsOnline = new JoinDataComplete(
            new JoinData
                {
                    AttributeName = "IsOnline",
                    JoinNumber = 1,
                    JoinSpan = 1
                },
            new JoinMetadata
                {
                    Description = "OnlineStatus",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

        [JoinName("Audio Name")] 
        public JoinDataComplete Name = new JoinDataComplete(
            new JoinData
                {
                    AttributeName = "Name",
                    JoinNumber = 1,
                    JoinSpan = 1
                },
            new JoinMetadata
                {
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial,
                    Description = "Name"                    
                });
        
        [JoinName("Send Command")] 
        public JoinDataComplete SendCommand = new JoinDataComplete(
            new JoinData
            {
                AttributeName = "SendCommand",
                JoinNumber = 1,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.FromSIMPL,
                JoinType = eJoinType.Serial,
                ValidValues = new[] { "clear", "boot", "reboot", "restart", "suspend", "resetKey", "wake" },
                Description = "Send Command"
            });

        [JoinName("Set Hostname")] 
        public JoinDataComplete SetHostname = new JoinDataComplete(
            new JoinData
            {
                AttributeName = "SetHostname",
                JoinNumber = 2,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.FromSIMPL,
                JoinType = eJoinType.Serial,
                Description = "SetHostname"
            });

        [JoinName("Set Password")] 
        public JoinDataComplete SetPassword = new JoinDataComplete(
            new JoinData
            {
                AttributeName = "SetPassword",
                JoinNumber = 3,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.FromSIMPL,
                JoinType = eJoinType.Serial,
                Description = "SetPassword"
            });

        [JoinName("Number Of Users")] 
        public JoinDataComplete NumberOfUsers = new JoinDataComplete(
            new JoinData
                {
                    AttributeName = "NumberOfUsers",
                    JoinNumber = 1,
                    JoinSpan = 1
                },
            new JoinMetadata
                {
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Analog,
                    Description = "Number Of Users"
                });

        public MersiveJoinMap(uint joinStart) : base(joinStart, typeof(MersiveJoinMap)) { }
    }
}