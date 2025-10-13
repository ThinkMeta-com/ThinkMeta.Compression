using System.IO.Compression;

namespace ThinkMeta.Compression;

/// <summary>
/// Provides static helper methods for compressing multiple files into ZIP archives.
/// Supports both synchronous and asynchronous operations, writing to streams or memory.
/// <para>
/// <b>Usage:</b> Each file is added to the ZIP archive as a separate entry using its file name.
/// </para>
/// <para>
/// <b>Thread Safety:</b> This class is static and stateless.
/// </para>
/// <para>
/// <b>Platform:</b> Requires .NET 9 and file system access; suitable for server-side scenarios.
/// </para>
/// </summary>
public static class ZipTools
{
    /// <summary>
    /// Compresses the specified files into a ZIP archive stored in a memory stream.
    /// Each file is added as a separate entry using its file name.
    /// </summary>
    /// <param name="files">The files to compress.</param>
    /// <returns>
    /// A <see cref="Stream"/> containing the ZIP archive. The stream's position is set to the beginning.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="files"/> is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown if any file does not exist.</exception>
    /// <exception cref="IOException">Thrown on I/O errors.</exception>
    public static Stream CompressFilesToMemoryStream(IEnumerable<FileInfo> files)
    {
        ArgumentNullException.ThrowIfNull(files);
        var stream = new MemoryStream();
        CompressFilesToStream(files, stream);
        _ = stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    /// <summary>
    /// Compresses the specified files into a ZIP archive written to the provided stream.
    /// Each file is added as a separate entry using its file name.
    /// </summary>
    /// <param name="files">The files to compress.</param>
    /// <param name="stream">The output stream for the ZIP archive.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="files"/> or <paramref name="stream"/> is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown if any file does not exist.</exception>
    /// <exception cref="IOException">Thrown on I/O errors.</exception>
    public static void CompressFilesToStream(IEnumerable<FileInfo> files, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentNullException.ThrowIfNull(stream);
        using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true);
        foreach (var file in files) {
            var zipEntry = zipArchive.CreateEntry(file.Name, CompressionLevel.Optimal);
            using var output = zipEntry.Open();
            using var input = file.OpenRead();
            input.CopyTo(output);
        }
    }

    /// <summary>
    /// Asynchronously compresses the specified files into a ZIP archive stored in a memory stream.
    /// Each file is added as a separate entry using its file name.
    /// </summary>
    /// <param name="files">The files to compress.</param>
    /// <returns>
    /// A <see cref="Task{Stream}"/> representing the asynchronous operation, with the ZIP archive in a memory stream.
    /// The stream's position is set to the beginning.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="files"/> is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown if any file does not exist.</exception>
    /// <exception cref="IOException">Thrown on I/O errors.</exception>
    public static async Task<Stream> CompressFilesToMemoryStreamAsync(IEnumerable<FileInfo> files)
    {
        ArgumentNullException.ThrowIfNull(files);
        var stream = new MemoryStream();
        await CompressFilesToStreamAsync(files, stream);
        _ = stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    /// <summary>
    /// Asynchronously compresses the specified files into a ZIP archive written to the provided stream.
    /// Each file is added as a separate entry using its file name.
    /// </summary>
    /// <param name="files">The files to compress.</param>
    /// <param name="stream">The output stream for the ZIP archive.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="files"/> or <paramref name="stream"/> is null.</exception>
    /// <exception cref="FileNotFoundException">Thrown if any file does not exist.</exception>
    /// <exception cref="IOException">Thrown on I/O errors.</exception>
    public static async Task CompressFilesToStreamAsync(IEnumerable<FileInfo> files, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentNullException.ThrowIfNull(stream);
        using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true);
        foreach (var file in files) {
            var zipEntry = zipArchive.CreateEntry(file.Name, CompressionLevel.Optimal);
            using var output = zipEntry.Open();
            using var input = file.OpenRead();
            await input.CopyToAsync(output);
        }
    }
}
