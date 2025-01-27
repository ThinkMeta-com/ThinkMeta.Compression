using System.IO.Compression;
using System.Linq.Expressions;

namespace ThinkMeta.Compression;

/// <summary>
/// Provides GZIP compression methods.
/// </summary>
public static class Gzip
{
    /// <summary>
    /// Compresses a byte array.
    /// </summary>
    /// <param name="data">The byte array.</param>
    /// <returns>A compressed byte array.</returns>
    public static byte[] Compress(byte[] data)
    {
        using var outputStream = new MemoryStream();

        var lengthBytes = BitConverter.GetBytes(data.Length);
        outputStream.Write(lengthBytes, 0, 4);

        using var compressorStream = new GZipStream(outputStream, CompressionLevel.Optimal);
        compressorStream.Write(data, 0, data.Length);
        compressorStream.Flush();

        return outputStream.ToArray();
    }

    /// <summary>
    /// Compresses an object.
    /// </summary>
    /// <typeparam name="T">The object's type.</typeparam>
    /// <param name="obj">The object.</param>
    /// <param name="byteSerializer">The serialization function.</param>
    /// <returns>A compressed byte array.</returns>
    public static byte[] Compress<T>(T obj, Func<T, byte[]> byteSerializer) => Compress(byteSerializer(obj));

    /// <summary>
    /// Compresses an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="byteSerializer"></param>
    /// <returns></returns>
    public static byte[] Compress<T>(T obj, Expression<Func<T, byte[]>> byteSerializer) => Compress(obj, byteSerializer.Compile());

    /// <summary>
    /// Decompresses a stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>The decompressed byte array.</returns>
    public static byte[] Decompress(Stream stream)
    {
        var lengthBytes = new byte[4];
        _ = stream.Read(lengthBytes, 0, 4);
        var length = BitConverter.ToInt32(lengthBytes, 0);

        using var compressorStream = new GZipStream(stream, CompressionMode.Decompress);
        var output = new byte[length];

        var bytesRead = 0;

        while (bytesRead < length)
            bytesRead += compressorStream.Read(output, bytesRead, length - bytesRead);

        return output;
    }

    /// <summary>
    /// Decompresses a byte array.
    /// </summary>
    /// <param name="data">The compressed byte array.</param>
    /// <returns>The decompressed byte array.</returns>
    public static byte[] Decompress(byte[] data)
    {
        using var stream = new MemoryStream(data);
        return Decompress(stream);
    }

    /// <summary>
    /// Decompresses an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="byteDeserializer"></param>
    /// <returns></returns>
    public static T Decompress<T>(Stream stream, Func<byte[], T> byteDeserializer) => byteDeserializer(Decompress(stream));

    /// <summary>
    /// Decompresses an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="byteDeserializer"></param>
    /// <returns></returns>
    public static T Decompress<T>(byte[] data, Func<byte[], T> byteDeserializer)
    {
        using var stream = new MemoryStream(data);
        return Decompress(stream, byteDeserializer);
    }

    /// <summary>
    /// Decompresses an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="byteDeserializer"></param>
    /// <returns></returns>
    public static T Decompress<T>(Stream stream, Expression<Func<byte[], T>> byteDeserializer) => Decompress(stream, byteDeserializer.Compile());

    /// <summary>
    /// Decompresses an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="byteDeserializer"></param>
    /// <returns></returns>
    public static T Decompress<T>(byte[] data, Expression<Func<byte[], T>> byteDeserializer) => Decompress(data, byteDeserializer.Compile());
}
