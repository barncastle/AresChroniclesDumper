namespace AresChroniclesDumper.FileSystem;

public struct VFileEntry
{
    public int VfileContentIndex { get; }

    public long Offset { get; }

    public int Length { get; }

    public VFileEntry(int vfileContentIndex, ulong offset, uint length)
    {
        VfileContentIndex = vfileContentIndex;
        Offset = (long)offset;
        Length = (int)length;
    }
}
