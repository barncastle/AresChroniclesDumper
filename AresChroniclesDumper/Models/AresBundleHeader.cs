namespace AresChroniclesDumper.Models;

/// <summary>
/// Header structure of AC's custom asset bundle
/// </summary>
internal class AresBundleHeader
{
    public const uint HeaderSize = 32;

    public string Signature = BundleConverter.BundleSignature;
    public uint Field1;
    public uint Field2;
    public uint Field3;
    public uint Field4;
    public uint Field5;
    public uint Field6;

    public AresBundleHeader()
    {
    }

    public AresBundleHeader(Stream stream)
    {
        Signature = stream.ReadCString();
        Field1 = stream.ReadUInt32BE();
        Field2 = stream.ReadUInt32BE();
        Field3 = stream.ReadUInt32BE();
        Field4 = stream.ReadUInt32BE();
        Field5 = stream.ReadUInt32BE();
        Field6 = stream.ReadUInt32BE();
    }

    public void Write(Stream stream)
    {
        stream.WriteCString(Signature);
        stream.WriteUInt32BE(Field1);
        stream.WriteUInt32BE(Field2);
        stream.WriteUInt32BE(Field3);
        stream.WriteUInt32BE(Field4);
        stream.WriteUInt32BE(Field5);
        stream.WriteUInt32BE(Field6);
    }
}
