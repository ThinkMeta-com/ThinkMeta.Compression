using System.Text;

namespace ThinkMeta.Compression;

/// <summary>
/// Extension class for (de)compressing strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Compresses a string with GZIP.
    /// </summary>
    /// <param name="s">The string to compress.</param>
    /// <returns>Compressed string as byte array.</returns>
    public static byte[] GzipCompress(this string s) => Gzip.Compress(Encoding.UTF8.GetBytes(s));

    /// <summary>
    /// Decompresses a GZIP byte array into a string
    /// </summary>
    /// <param name="data">The byte array.</param>
    /// <returns>A string.</returns>
    public static string GzipDecompress(byte[] data) => Encoding.UTF8.GetString(Gzip.Decompress(data));
}
