using System.Buffers.Binary;
using System.Text;

namespace AresChroniclesDumper;

public static class StreamExtensions
{
    private static readonly byte[] Buffer = new byte[0x100];

    public static uint ReadUInt32BE(this Stream stream)
    {
        stream.Read(Buffer, 0, 4);
        return BinaryPrimitives.ReadUInt32BigEndian(Buffer);
    }

    public static ulong ReadUInt64BE(this Stream stream)
    {
        stream.Read(Buffer, 0, 8);
        return BinaryPrimitives.ReadUInt64BigEndian(Buffer);
    }

    public static string ReadCString(this Stream stream)
    {
        int b, i = 0;
        while ((b = stream.ReadByte()) > 0 && i < 0x100)
            Buffer[i++] = (byte)b;

        return Encoding.UTF8.GetString(Buffer, 0, i);
    }

    public static void WriteUInt32BE(this Stream stream, uint value)
    {
        BinaryPrimitives.WriteUInt32BigEndian(Buffer, value);
        stream.Write(Buffer, 0, 4);
    }

    public static void WriteUInt64BE(this Stream stream, ulong value)
    {
        BinaryPrimitives.WriteUInt64BigEndian(Buffer, value);
        stream.Write(Buffer, 0, 8);
    }

    public static void WriteCString(this Stream stream, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var size = Encoding.UTF8.GetBytes(value, Buffer);
            stream.Write(Buffer, 0, size);
        }

        stream.WriteByte(0);
    }
}
