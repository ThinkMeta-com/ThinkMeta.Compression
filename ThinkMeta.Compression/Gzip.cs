using System.IO.Compression;
using System.Linq.Expressions;

namespace ThinkMeta.Compression;

/// <summary>
/// Provides static methods for compressing and decompressing data using the GZIP algorithm.
/// Supports compression and decompression of raw byte arrays, streams, and objects via custom serialization and deserialization delegates.
/// <para>
/// <b>Thread Safety:</b> This class is static and stateless.
/// </para>
/// <para>
/// <b>Usage:</b> The compressed format stores the original length in the first 4 bytes (little-endian), followed by the GZIP-compressed data.
/// </para>
/// </summary>
public static class Gzip
{
    /// <summary>
    /// Compresses a byte array using GZIP.
    /// The output format is: [4 bytes original length][GZIP compressed data].
    /// The first 4 bytes store the original uncompressed length (little-endian).
    /// </summary>
    /// <param name="data">The byte array to compress.</param>
    /// <returns>
    /// A byte array containing the original length followed by the GZIP-compressed data.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
    public static byte[] Compress(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var outputStream = new MemoryStream();

        var lengthBytes = BitConverter.GetBytes(data.Length);
        outputStream.Write(lengthBytes, 0, 4);

        using var compressorStream = new GZipStream(outputStream, CompressionLevel.Optimal);
        compressorStream.Write(data, 0, data.Length);
        compressorStream.Flush();

        return outputStream.ToArray();
    }

    /// <summary>
    /// Compresses an object using a custom serialization function.
    /// </summary>
    /// <typeparam name="T">The type of the object to compress.</typeparam>
    /// <param name="obj">The object to compress.</param>
    /// <param name="byteSerializer">A delegate that serializes the object to a byte array.</param>
    /// <returns>A GZIP-compressed byte array containing the serialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="byteSerializer"/> is null.</exception>
    public static byte[] Compress<T>(T obj, Func<T, byte[]> byteSerializer)
    {
        ArgumentNullException.ThrowIfNull(byteSerializer);
        return Compress(byteSerializer(obj));
    }

    /// <summary>
    /// Compresses an object using a custom serialization expression.
    /// </summary>
    /// <typeparam name="T">The type of the object to compress.</typeparam>
    /// <param name="obj">The object to compress.</param>
    /// <param name="byteSerializer">An expression that serializes the object to a byte array.</param>
    /// <returns>A GZIP-compressed byte array containing the serialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="byteSerializer"/> is null.</exception>
    public static byte[] Compress<T>(T obj, Expression<Func<T, byte[]>> byteSerializer)
    {
        ArgumentNullException.ThrowIfNull(byteSerializer);
        return Compress(obj, byteSerializer.Compile());
    }

    /// <summary>
    /// Decompresses a stream containing GZIP-compressed data with a 4-byte length prefix.
    /// </summary>
    /// <param name="stream">The stream containing the compressed data.</param>
    /// <returns>The decompressed byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown if the stream does not contain valid GZIP data.</exception>
    public static byte[] Decompress(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
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
    /// Decompresses a byte array containing GZIP-compressed data with a 4-byte length prefix.
    /// </summary>
    /// <param name="data">The compressed byte array.</param>
    /// <returns>The decompressed byte array.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown if the data does not contain valid GZIP data.</exception>
    public static byte[] Decompress(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var stream = new MemoryStream(data);
        return Decompress(stream);
    }

    /// <summary>
    /// Decompresses a stream and deserializes the result using a custom deserialization function.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="stream">The stream containing the compressed data.</param>
    /// <param name="byteDeserializer">A delegate that deserializes the decompressed byte array to an object.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="byteDeserializer"/> is null.</exception>
    public static T Decompress<T>(Stream stream, Func<byte[], T> byteDeserializer)
    {
        ArgumentNullException.ThrowIfNull(byteDeserializer);
        return byteDeserializer(Decompress(stream));
    }

    /// <summary>
    /// Decompresses a byte array and deserializes the result using a custom deserialization function.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="data">The compressed byte array.</param>
    /// <param name="byteDeserializer">A delegate that deserializes the decompressed byte array to an object.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="byteDeserializer"/> is null.</exception>
    public static T Decompress<T>(byte[] data, Func<byte[], T> byteDeserializer)
    {
        ArgumentNullException.ThrowIfNull(byteDeserializer);
        using var stream = new MemoryStream(data);
        return Decompress(stream, byteDeserializer);
    }

    /// <summary>
    /// Decompresses a stream and deserializes the result using a custom deserialization expression.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="stream">The stream containing the compressed data.</param>
    /// <param name="byteDeserializer">An expression that deserializes the decompressed byte array to an object.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="byteDeserializer"/> is null.</exception>
    public static T Decompress<T>(Stream stream, Expression<Func<byte[], T>> byteDeserializer)
    {
        ArgumentNullException.ThrowIfNull(byteDeserializer);
        return Decompress(stream, byteDeserializer.Compile());
    }

    /// <summary>
    /// Decompresses a byte array and deserializes the result using a custom deserialization expression.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="data">The compressed byte array.</param>
    /// <param name="byteDeserializer">An expression that deserializes the decompressed byte array to an object.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="byteDeserializer"/> is null.</exception>
    public static T Decompress<T>(byte[] data, Expression<Func<byte[], T>> byteDeserializer)
    {
        ArgumentNullException.ThrowIfNull(byteDeserializer);
        return Decompress(data, byteDeserializer.Compile());
    }
}
