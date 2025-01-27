using System.IO.Compression;

namespace ThinkMeta.Compression;

/// <summary>
/// Tools for ZIP compression.
/// </summary>
public static class ZipTools
{
    /// <summary>
    /// Compresses files to ZIP in memory.
    /// </summary>
    /// <param name="files">List of files.</param>
    /// <returns></returns>
    public static Stream CompressFilesToMemoryStream(IEnumerable<FileInfo> files)
    {
        var stream = new MemoryStream();
        using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true)) {
            foreach (var file in files) {
                var zipEntry = zipArchive.CreateEntry(file.Name, CompressionLevel.Optimal);
                using var output = zipEntry.Open();
                using var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                var bytes = new byte[fileStream.Length];
                fileStream.ReadExactly(bytes, 0, bytes.Length);
                output.Write(bytes);
            }
        }

        _ = stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}
