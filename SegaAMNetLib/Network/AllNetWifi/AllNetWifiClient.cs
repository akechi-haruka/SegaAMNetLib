using System.Net;
using System.Net.Sockets;
using System.Text;
using Haruka.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Haruka.Arcade.SegaAMNetLib.Network.AllNetWifi;

public class AllNetWifiClient {
    private static readonly ILogger LOG = Log.GetOrCreate("AllnetWifi");

    private const int PORT = 40010;

    public WifiStatus Status {
        get {
            if (status?.IsValid() ?? false) {
                return status;
            }

            return null;
        }
    }

    private UdpClient client;
    private Thread thread;
    private WifiStatus status;
    private bool running;

    public void Start() {
        if (running) {
            return;
        }

        running = true;
        thread = new Thread(ExecuteT);
        thread.Start();
    }

    private void ExecuteT() {
        LOG.LogInformation("All.Net Wifi receiver started");
        client = new UdpClient(PORT, AddressFamily.InterNetwork) {
            EnableBroadcast = true
        };
        while (running) {
            try {
                LOG.LogTrace("Received packet");
                IPEndPoint _ = null;
                status = new WifiStatus(JsonConvert.DeserializeObject<DataPacket>(Encoding.UTF8.GetString(client.Receive(ref _))));
                LOG.LogTrace(status.AuthServerStatus + ", " + status.WifiServerStatus);
            } catch {
                // ignored
            }
        }

        LOG.LogInformation("All.Net Wifi receiver stopped");
    }

    public void Stop() {
        if (running) {
            running = false;
            client?.Close();
            thread?.Join();
            thread = null;
        }
    }

    public class WifiStatus {
        public bool AuthServerStatus { get; }
        public bool WifiServerStatus { get; }
        public String Serial { get; }
        public String FirmwareVersion { get; }
        public bool IsServer { get; }
        public int ClientCount { get; }
        public int LifeTime { get; }
        public String ServerSerial { get; }
        public DateTime Received { get; }

        internal WifiStatus(DataPacket p) {
            AuthServerStatus = p.auth;
            WifiServerStatus = p.wifi_server;
            Serial = p.serial;
            FirmwareVersion = p.firm_version;
            IsServer = p.wifi_server;
            ClientCount = p.wifi_clients;
            LifeTime = p.lifetime;
            ServerSerial = p.master_serial;
            Received = DateTime.Now;
        }

        public bool IsValid() {
            return DateTime.Now < Received + TimeSpan.FromSeconds(LifeTime);
        }
    }
}