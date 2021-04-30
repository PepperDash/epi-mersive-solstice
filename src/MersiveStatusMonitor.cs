using System;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace MersiveEpi
{
    public class MersiveStatusMonitor : StatusMonitorBase
    {
        private readonly int _pollTime;
        private readonly int _timeToWarning;
        private readonly int _timeToError;

        private CTimer _timer;
        public event EventHandler<GenericHttpClientEventArgs> ResponseReceived;

        public MersiveStatusMonitor(IKeyed parent, CommunicationMonitorConfig config)
            : base(parent, config.TimeToWarning, config.TimeToError)
        {
            _pollTime = config.PollInterval == default ( int ) ? 30000 : config.PollInterval;
            _timeToWarning = config.TimeToWarning;
            _timeToError = config.TimeToError;
        }

        public override void Start()
        {
            if (_timer == null)
                _timer = new CTimer(PollTimerCallback, null, _pollTime, _pollTime);

            _timer.Reset(_pollTime, _pollTime);
            StartErrorTimers();
        }

        public override void Stop()
        {
            if (_timer != null)
                _timer.Stop();

            StopErrorTimers();
        }

        private void PollTimerCallback(object o)
        {
            try
            {
                if (string.IsNullOrEmpty(Hostname))
                    return;

                var url = GetUrl();

                var request = new HttpClientRequest
                {
                    RequestType = RequestType.Get
                };

                request.Url.Parse(url);

                using (var client = new HttpClient())
                using (var response = client.Dispatch(request))
                {
                    SetOk();
                    OnResponseReceived(response);
                }
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

        private string GetUrl()
        {
            var url = string.Format("http://{0}/api/stats", Hostname);
            if (!string.IsNullOrEmpty(Password))
                url = string.Format("{0}?password={1}", url, Password);

            return url;
        }

        public string Hostname { private get; set; }
        public string Password { private get; set; }

        public CommunicationMonitorConfig Config
        {
            get
            {
                return new CommunicationMonitorConfig
                    {
                        PollInterval = _pollTime,
                        TimeToError = _timeToError,
                        TimeToWarning = _timeToWarning,
                        PollString = "api/stats"
                    };
            }
        }

        private void OnResponseReceived(HttpClientResponse response)
        {
            var handler = ResponseReceived;
            if (handler == null)
                return;

            handler.Invoke(this, new GenericHttpClientEventArgs(response.ContentString, response.ResponseUrl, HTTP_CALLBACK_ERROR.COMPLETED));
        }

        private void SetOk()
        {
            Status = MonitorStatus.IsOk;
            ResetErrorTimers();
        }
    }
}