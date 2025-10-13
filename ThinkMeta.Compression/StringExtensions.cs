using System.Text;

namespace ThinkMeta.Compression;

/// <summary>
/// Extension methods for compressing and decompressing strings using GZIP with UTF-8 encoding.
/// The compressed format is a byte array: [4 bytes original length][GZIP compressed data].
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Compresses a string using GZIP and UTF-8 encoding.
    /// The output is a byte array containing the original length (4 bytes, little-endian) followed by GZIP-compressed data.
    /// </summary>
    /// <param name="s">The string to compress.</param>
    /// <returns>Compressed data as a byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="s"/> is null.</exception>
    public static byte[] GzipCompress(this string s)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Gzip.Compress(Encoding.UTF8.GetBytes(s));
    }

    /// <summary>
    /// Decompresses a GZIP-compressed byte array (with a 4-byte length prefix) into a string using UTF-8 encoding.
    /// </summary>
    /// <param name="data">The compressed byte array.</param>
    /// <returns>The decompressed string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
    public static string GzipDecompress(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        return Encoding.UTF8.GetString(Gzip.Decompress(data));
    }
}
