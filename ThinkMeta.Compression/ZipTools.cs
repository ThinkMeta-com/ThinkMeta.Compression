using System.IO.Compression;

namespace ThinkMeta.Compression;

/// <summary>
/// Tools for ZIP compression.
/// </summary>
public static class ZipTools
{
    /// <summary>
    /// Compresses files as ZIP into a memory stream.
    /// </summary>
    /// <param name="files">List of files.</param>
    /// <returns></returns>
    public static Stream CompressFilesToMemoryStream(IEnumerable<FileInfo> files)
    {
        var stream = new MemoryStream();
        CompressFilesToStream(files, stream);
        _ = stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    /// <summary>
    /// Compresses files as ZIP into a stream.
    /// </summary>
    /// <param name="files">List of files.</param>
    /// <param name="stream">The stream.</param>
    public static void CompressFilesToStream(IEnumerable<FileInfo> files, Stream stream)
    {
        using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true);
        foreach (var file in files) {
            var zipEntry = zipArchive.CreateEntry(file.Name, CompressionLevel.Optimal);
            using var output = zipEntry.Open();
            var bytes = File.ReadAllBytes(file.FullName);
            output.Write(bytes);
        }
    }

    /// <summary>
    /// Compresses files as ZIP into a memory stream.
    /// </summary>
    /// <param name="files">List of files.</param>
    /// <returns></returns>
    public static async Task<Stream> CompressFilesToMemoryStreamAsync(IEnumerable<FileInfo> files)
    {
        var stream = new MemoryStream();
        await CompressFilesToStreamAsync(files, stream).ConfigureAwait(false);
        _ = stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    /// <summary>
    /// Compresses files as ZIP into a stream.
    /// </summary>
    /// <param name="files">List of files.</param>
    /// <param name="stream">The stream.</param>
    public static async Task CompressFilesToStreamAsync(IEnumerable<FileInfo> files, Stream stream)
    {
        using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true);
        foreach (var file in files) {
            var zipEntry = zipArchive.CreateEntry(file.Name, CompressionLevel.Optimal);
            using var output = zipEntry.Open();
            var bytes = await File.ReadAllBytesAsync(file.FullName);
            await output.WriteAsync(bytes).ConfigureAwait(false);
        }
    }
}
