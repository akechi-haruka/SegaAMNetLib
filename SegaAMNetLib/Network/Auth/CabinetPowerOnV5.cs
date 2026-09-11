using System.Collections.Specialized;
using Haruka.Arcade.SegaAMNetLib.Constants;

namespace Haruka.Arcade.SegaAMNetLib.Network.Auth;

public class CabinetPowerOnV5 : AllNetRequest {
    private readonly String token;

    public CabinetPowerOnV5(string hostname, String gameID, String gameVersion, String keychipID, String token = "dummy") : base("http://" + hostname + "/sys/servlet/PowerOn", new Dictionary<string, string>() {
        { "title_id", gameID },
        { "title_ver", gameVersion },
        { "machine", keychipID },
        { "firm_ver", "60000" },
        { "boot_ver", "1000" },
        { "encode", "UTF-8" },
        { "format", "5.00" },
        { "hops", "1" },
        { "token", token }
    }) {
        this.token = token;
    }

    public AuthResult ExecuteAuthentication() {
        NameValueCollection resp = Execute();

        if (resp["token"] != token) {
            throw new Exception("Token mismatch, expected " + token + ", got " + resp["token"]);
        }

        return new AuthResult(resp);
    }

    public class AuthResult {
        public AuthResult(NameValueCollection resp) {
            Result = (Result)Int32.Parse(resp["result"]);
            if (Result == Result.Success) {
                PlaceId = resp["place_id"];
                GameServerUri = resp["title_uri"];
                GameServerHost = resp["title_host"];
                StoreName = resp["name"];
                StoreNickname = resp["nickname"];
                Setting = Int32.Parse(resp["setting"]);
                Prefecture = (Prefecture)Int32.Parse(resp["region0"]);
                RegionName = (resp["region_name0"] + " " + resp["region_name1"] + " " + resp["region_name2"] + resp["region_name3"]).Trim();
                CountryCode = resp["country"];
                AllNetId = Int32.Parse(resp["allnet_id"]);
                AuthTimeUtc = DateTime.Parse(resp["utc_time"]);
                AuthTimeLocal = AuthTimeUtc + new TimeSpan(Int32.Parse(resp["client_timezone"].Substring(0, 3)), Int32.Parse(resp["client_timezone"].Substring(3)), 0);
                LocationIp = resp["location_ip"];
            }
        }

        public Result Result { get; }
        public String PlaceId { get; }
        public String GameServerUri { get; }
        public String GameServerHost { get; }
        public String StoreName { get; }
        public String StoreNickname { get; }
        public int Setting { get; }
        public Prefecture Prefecture { get; }
        public String RegionName { get; }
        public String CountryCode { get; }
        public int AllNetId { get; }
        public DateTime AuthTimeUtc { get; }
        public DateTime AuthTimeLocal { get; }
        public String LocationIp { get; }
    }

    public enum Result {
        Success = 1,
        GameError = -1,
        BoardError = -2,
        LocationError = -3
    }
}