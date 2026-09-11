using System.Text;

namespace Haruka.Arcade.SegaAMNetLib.Util;

public static class HttpContentExtensions {
    extension(HttpContent resp) {
        public string ReadString(Encoding encoding = null) {
            return (encoding ?? Encoding.UTF8).GetString(resp.ReadBytes());
        }

        public byte[] ReadBytes() {
            Stream responseStream = resp.ReadAsStream();
            using MemoryStream memoryStream = new MemoryStream();
            responseStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}