using Microsoft.Extensions.Logging;

namespace Haruka.Arcade.SegaAMNetLib.Network.Auth;

public class ServiceHealth : WebRequest {
    public ServiceHealth(String hostname) : base("http://" + hostname + "/sys/servlet/Alive", null) {
    }

    public bool ExecuteCheckAlive() {
        try {
            return Execute() == "OK";
        } catch (Exception ex) {
            LOG.LogWarning(ex, "Alive check failed");
            return false;
        }
    }
}