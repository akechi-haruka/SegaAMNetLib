using System.Buffers.Text;
using System.IO.Compression;
using System.Text;

namespace Haruka.Arcade.SegaAMNetLib.Util;

public static class Dfi {
    public static byte[] Encode(string str) {
        return Compress(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(str))));
    }

    public static string Decode(byte[] bytes) {
        return Encoding.UTF8.GetString(Convert.FromBase64String(Encoding.UTF8.GetString(Decompress(bytes))));
    }

    private static byte[] Compress(byte[] data) {
        MemoryStream output = new MemoryStream();
        using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal)) {
            dstream.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    private static byte[] Decompress(byte[] data) {
        MemoryStream input = new MemoryStream(data);
        MemoryStream output = new MemoryStream();
        using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress)) {
            dstream.CopyTo(output);
        }

        return output.ToArray();
    }
}