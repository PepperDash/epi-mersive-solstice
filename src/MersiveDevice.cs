using System;
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharpPro.DeviceSupport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.Devices;

namespace MersiveEpi
{
    public class MersiveSolsticeDevice : ReconfigurableBridgableDevice, ICommunicationMonitor, IOnline, IRoutingSource
    {
        private readonly MersiveStatusMonitor _monitor;
        private MersiveStatsResponse _currentStats;
       
        private string _hostname;
        private string _password;

        public StringFeedback DisplayName { get; private set; }
        public StringFeedback DisplayId { get; private set; }
        public IntFeedback ConnectedUsers { get; private set; }

        public MersiveSolsticeDevice(DeviceConfig config)
            : base(config)
        {
            var props = MersiveConfig.FromDeviceConfig(config);
            if (props.CommunicationMonitor == null)
                props.CommunicationMonitor = new CommunicationMonitorConfig
                    {
                        PollInterval = 10000,
                        TimeToWarning = 60000,
                        TimeToError = 120000,
                    };

            _hostname = props.Control.TcpSshProperties.Hostname;
            _password = props.Control.TcpSshProperties.Password;

            _monitor = new MersiveStatusMonitor(this, props.CommunicationMonitor)
                {
                    Hostname = _hostname,
                    Password = _password,
                };

            _monitor.ResponseReceived += MonitorOnResponseReceived;

            OutputPorts = new RoutingPortCollection<RoutingOutputPort>
                {
                    new RoutingOutputPort("hdmiOut",
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Hdmi,
                        null,
                        this)
                };
        }

        public override bool CustomActivate()
        {
            DisplayName = new StringFeedback("DisplayName",
                () => _currentStats == null ? string.Empty : _currentStats.DisplayInformation.DisplayName);

            DisplayName.OutputChange +=
                (sender, args) => Debug.Console(1, this, "Display Name Updated: {0}", args.StringValue);

            DisplayId = new StringFeedback("DisplayId",
                () => _currentStats == null ? string.Empty : _currentStats.DisplayInformation.ProductVariant);

            DisplayId.OutputChange +=
                (sender, args) => Debug.Console(1, this, "Display Id Updated: {0}", args.StringValue);

            ConnectedUsers = new IntFeedback("ConnectedUsers",
                () => _currentStats == null ? 0 : (int) _currentStats.Statistics.CurrentLiveSourceCount);

            DisplayId.OutputChange +=
                (sender, args) => Debug.Console(1, this, "Number of Connected Users Updated: {0}", args.UShortValue);

            _monitor.Start();
            return base.CustomActivate();
        }

        private void MonitorOnResponseReceived(object sender, GenericHttpClientEventArgs args)
        {
            _currentStats = JsonConvert.DeserializeObject<MersiveStatsResponse>(args.ResponseText);

            DisplayName.FireUpdate();
            DisplayId.FireUpdate();
            ConnectedUsers.FireUpdate();
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new MersiveJoinMap(joinStart);
            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            //Debug.Console(0, this, "Linking {0} at join : {1} | joinmap:{2}", Name, joinStart, joinMap.IsOnline.JoinNumber);
            IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.IsOnline.JoinNumber]);
            ConnectedUsers.LinkInputSig(trilist.UShortInput[joinMap.NumberOfUsers.JoinNumber]);
            DisplayName.LinkInputSig(trilist.StringInput[joinMap.Name.JoinNumber]);
            DisplayId.LinkInputSig(trilist.StringInput[joinMap.Id.JoinNumber]);

            trilist.SetStringSigAction(joinMap.SetHostname.JoinNumber, SetHostname);
            trilist.SetStringSigAction(joinMap.SetPassword.JoinNumber, SetPassword);
            trilist.SetStringSigAction(joinMap.SendCommand.JoinNumber, DispatchCommand);
        }

        public void DispatchCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            Debug.Console(1, this, "Sending command {0}", command);
            if (string.IsNullOrEmpty(_hostname))
            {
                Debug.Console(1, this, "Can't send command {0} | hostname is empty", command);
                return;
            }     

            try
            {
                var request = new HttpClientRequest();
                var url = string.Format("http://{0}/api/control/{1}", _hostname, command);
                if (!string.IsNullOrEmpty(_password))
                    url = string.Format("{0}?password={1}", url, _password);

                request.Url.Parse(url);
                using (var client = new HttpClient())
                    client.Dispatch(request);
            }
            catch (HttpException ex)
            {
                Debug.Console(1, this, "Error communicating with device {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error {0} {1}", ex.Message, ex.StackTrace);
            }
        }

        public void SetHostname(string hostname)
        {
            if (string.IsNullOrEmpty(hostname) || _hostname.Equals(hostname))
                return;


            Debug.Console(1, this, "Setting hostname {0}", hostname);
            _monitor.Hostname = hostname;
            _hostname = hostname;
            UpdateConfig();
        }

        public void SetPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || _hostname.Equals(password))
                return;

            Debug.Console(1, this, "Setting password {0}", password);
            _monitor.Password = password;
            _password = password;
            UpdateConfig();
        }

        private void UpdateConfig()
        {
            var props = new MersiveConfig
                {
                    CommunicationMonitor = _monitor.Config,
                    Control = new MersiveConfig.MersiveControlPropertiesConfig
                        {
                            Method = "http",
                            TcpSshProperties = new MersiveConfig.MersiveControlPropertiesDetails
                                {
                                    Hostname = _hostname,
                                    Password = string.IsNullOrEmpty(_password) ? string.Empty : _password
                                }
                        }
                };

            var config = new DeviceConfig
                {
                    Group = "wireless",
                    Key = Key,
                    Name = Name,
                    Type = "MersiveSolstice",
                    Uid = (int) new Random().NextDouble(),
                    Properties = JObject.FromObject(props)
                };

            SetConfig(config);
        }     

        public StatusMonitorBase CommunicationMonitor 
        {
            get { return _monitor; }
        }

        public BoolFeedback IsOnline
        {
            get { return _monitor.IsOnlineFeedback; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }
    }
}

