namespace AresChroniclesDumper.Models;

/// <summary>
/// Header structure of Unity's asset bundle
/// </summary>
internal class UnityBundleHeader
{
    public uint HeaderSize => 34u + (uint)(UnityVersion.Length + UnityRevision.Length);

    public string Signature = BundleConverter.BundleSignature;
    public uint Version = 6;
    public string UnityVersion = "5.x.x";
    public string UnityRevision = "2018.4.18f1";
    public ulong Size;
    public uint CompressedBlocksInfoSize;
    public uint UncompressedBlocksInfoSize;
    public uint Flags;

    public UnityBundleHeader()
    {
    }

    public UnityBundleHeader(Stream stream)
    {
        Signature = stream.ReadCString();
        Version = stream.ReadUInt32BE();
        UnityVersion = stream.ReadCString();
        UnityRevision = stream.ReadCString();
        Size = stream.ReadUInt64BE();
        CompressedBlocksInfoSize = stream.ReadUInt32BE();
        UncompressedBlocksInfoSize = stream.ReadUInt32BE();
        Flags = stream.ReadUInt32BE();
    }

    public void Write(Stream stream)
    {
        stream.WriteCString(Signature);
        stream.WriteUInt32BE(Version);
        stream.WriteCString(UnityVersion);
        stream.WriteCString(UnityRevision);
        stream.WriteUInt64BE(Size);
        stream.WriteUInt32BE(CompressedBlocksInfoSize);
        stream.WriteUInt32BE(UncompressedBlocksInfoSize);
        stream.WriteUInt32BE(Flags);
    }
}
