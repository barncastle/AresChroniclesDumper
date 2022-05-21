using FlatBuffers;

namespace AresChroniclesDumper.FileSystem;

internal class ZeusFileSystem : IDisposable
{
    public const string VFileIndex = "_vfileIndexV2.fb";
    public const string VFileContent = "_vfileContent";

    private readonly string ROOT_DIRECTORY;
    private Dictionary<string, VFileEntry> _vFileEntrys;
    private string[] _VFileContentPath;
    private Stream[] _VFileContentStreams;

    public ZeusFileSystem(string root)
    {
        ROOT_DIRECTORY = root;
    }

    public void LoadVFileEntrysFromFB()
    {
        var buffer = File.ReadAllBytes(Path.Combine(ROOT_DIRECTORY, VFileIndex));
        var rootAsVFileContentFB = VFileContentFB.GetRootAsVFileContentFB(new ByteBuffer(buffer));
        var entrysLength = rootAsVFileContentFB.EntrysLength;

        _VFileContentPath = new string[entrysLength];
        _VFileContentStreams = new Stream[entrysLength];
        _vFileEntrys = new Dictionary<string, VFileEntry>(rootAsVFileContentFB.FileEntryCount);

        for (var i = 0; i < entrysLength; i++)
        {
            var value = rootAsVFileContentFB.Entrys(i).Value;
            var contentFile = value.ContentFile;
            var filePath = Path.Combine(ROOT_DIRECTORY, contentFile);

            _VFileContentPath[i] = filePath;

            if (File.Exists(filePath))
                _VFileContentStreams[i] = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            for (var j = 0; j < value.EntrysLength; j++)
            {
                var value2 = value.Entrys(j).Value;
                var value3 = new VFileEntry(i, value2.Offset, value2.Length);
                _vFileEntrys.Add(value2.Path, value3);
            }
        }
    }

    public IEnumerable<string> GetFileEntries()
    {
        return _vFileEntrys.Keys;
    }

    public Stream OpenReadStream(string virtualPath)
    {
        if (TryGetValueFromVFileContent(virtualPath, out VFileEntry entry))
        {
            var baseStream = _VFileContentStreams[entry.VfileContentIndex];

            if (baseStream != null)
            {
                return new SubStream(baseStream, entry.Offset, entry.Length);
            }
        }

        Console.WriteLine($"[INFO] Can't find file \"{virtualPath}\".");
        return null;
    }

    private bool TryGetValueFromVFileContent(string virtualPath, out VFileEntry entry)
    {
        return _vFileEntrys.TryGetValue(virtualPath.TrimStart('\\'), out entry);
    }

    public void Dispose()
    {
        Array.ForEach(_VFileContentStreams, s => s?.Dispose());
    }
}
