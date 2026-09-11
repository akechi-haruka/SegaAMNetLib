using System.Net.NetworkInformation;
using Haruka.Common;
using Microsoft.Extensions.Logging;

namespace Haruka.Arcade.SegaAMNetLib.Util;

public static class IcmpPing {
    private static readonly ILogger LOG = Log.GetOrCreate("Icmp");

    public static bool PingHost(string nameOrAddress) {
        LOG.LogInformation("Pinging: " + nameOrAddress);
        bool pingable = false;
        Ping pinger = null;

        try {
            pinger = new Ping();
            PingReply reply = pinger.Send(nameOrAddress);
            pingable = reply?.Status == IPStatus.Success;
            LOG.LogInformation("Ping Result: " + reply?.Status + " / RTT: " + reply?.RoundtripTime);
        } catch {
            // Discard PingExceptions and return false;
            LOG.LogWarning("Ping failed");
        } finally {
            pinger?.Dispose();
        }

        return pingable;
    }
}