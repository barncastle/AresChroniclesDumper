using AresChroniclesDumper.FileSystem;

namespace AresChroniclesDumper;

static class Program
{
    private const string OutputFolder = "Dump";
    private const string AssetFolder = @"zskrpc_Data\StreamingAssets\";

    public static void Main()
    {
#if DEBUG
        Directory.SetCurrentDirectory(@"C:\zsyjkr\client");
#endif

        if (CheckForRequirements())
        {
            DumpInnerPackageAssets();
            DumpOuterPackageAssets();
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();
    }

    private static bool CheckForRequirements()
    {
        if (!Directory.Exists(AssetFolder))
        {
            Console.WriteLine($"[ERROR] Unable to locate the \"{AssetFolder}\" folder.");
            return false;
        }

        if (!File.Exists(Path.Combine(AssetFolder, ZeusFileSystem.VFileIndex)))
        {
            Console.WriteLine($"[ERROR] Unable to locate the \"{ZeusFileSystem.VFileIndex}\" file.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Inner Package Assets refer to those contained
    /// within the base game's _vfileContent archives
    /// </summary>
    private static void DumpInnerPackageAssets()
    {
        using var fileSystem = new ZeusFileSystem(AssetFolder);

        fileSystem.LoadVFileEntrysFromFB();

        foreach (var fileEntry in fileSystem.GetFileEntries())
        {
            using var stream = fileSystem.OpenReadStream(fileEntry);

            if (stream == null)
                continue;

            Console.WriteLine($"Dumping InnerPackage {fileEntry}...");
            DumpAsset(fileEntry, "InnerPackage", stream);
        }
    }

    /// <summary>
    /// Outer Package Assets refer to those downloaded
    /// via updates and hotfixes
    /// </summary>
    private static void DumpOuterPackageAssets()
    {
        var bundlesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low",
            "PerfectWorld",
            "아레스 크로니클",
            "OuterPackage",
            "Bundles");

        if (!Directory.Exists(bundlesPath))
            return;

        foreach (var bundle in Directory.GetFiles(bundlesPath, "*.unity3d", SearchOption.AllDirectories))
        {
            using var stream = File.OpenRead(bundle);

            var fileEntry = bundle[bundle.IndexOf("Bundles")..];
            Console.WriteLine($"Dumping OuterPackage {fileEntry}...");
            DumpAsset(fileEntry, "OuterPackage", stream);
        }
    }

    /// <summary>
    /// Copies an asset to the <see cref="OutputFolder"/> and
    /// converts it to a normal Asset Bundle if required
    /// </summary>
    /// <param name="fileEntry"></param>
    /// <param name="stream"></param>
    private static void DumpAsset(string fileEntry, string subfolder, Stream stream)
    {
        var filename = Path.Combine(OutputFolder, subfolder, fileEntry);
        var directory = Path.GetDirectoryName(filename);

        Directory.CreateDirectory(directory);

        using var fs = File.Create(filename);
        stream.CopyTo(fs);
        fs.Flush();

        if (BundleConverter.IsAresBundle(fs))
            BundleConverter.ConvertAresToUnity(fs);
    }
}