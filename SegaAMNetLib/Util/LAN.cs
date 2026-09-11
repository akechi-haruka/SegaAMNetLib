using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Haruka.Common;
using Microsoft.Extensions.Logging;

namespace Haruka.Arcade.SegaAMNetLib.Util;

public static class Lan {
    private static readonly ILogger LOG = Log.GetOrCreate("LAN");

    public static String ExternalIp { get; private set; }

    public static bool IsConnected() {
        return NetworkInterface.GetIsNetworkAvailable();
    }

    public static String GetPrimaryLanIp() {
        string localIp = null;
        try {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0)) {
                socket.Connect("9.9.9.9", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIp = endPoint.Address.ToString();
            }
        } catch (Exception ex) {
            LOG.LogError(ex, "Failed to get primary LAN IP");
        }

        return localIp;
    }

    public static String[] GetLanIpAddresses() {
        List<String> ips = new List<string>();
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                ips.Add(ip.ToString());
            }
        }

        return ips.ToArray();
    }

    public static NetworkInterface GetPrimaryAdapter() {
        String primary = GetPrimaryLanIp();
        return NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(na => na.GetIPProperties().UnicastAddresses.FirstOrDefault(var => var.Address.ToString() == primary) != null);
    }

    public static NetworkInterface[] GetAdapters() {
        return NetworkInterface.GetAllNetworkInterfaces();
    }

    public static UnicastIPAddressInformation GetAdapterIpV4Properties(NetworkInterface ni) {
        return ni.GetIPProperties().UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);
    }

    public static bool IsPrimaryAdapterUsingDhcp() {
        return GetPrimaryAdapter().GetIPProperties().GetIPv4Properties().IsDhcpEnabled;
    }

    public static string GetPrimaryMacAddress() {
        return String.Join('-', GetPrimaryAdapter().GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
    }
}