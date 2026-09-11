using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

namespace Haruka.Arcade.SegaAMNetLib.Network.AllNetWifi;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[UsedImplicitly]
class DataPacket {
    public bool auth;
    public String serial;
    public String firm_version;
    public bool wifi_server;
    public int wifi_clients;
    public int lifetime;
    public String master_serial;
}