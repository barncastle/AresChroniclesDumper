using System.Runtime.CompilerServices;
using System.Text;

namespace AresChroniclesDumper;

public static class StreamExtensions
{
    private static readonly byte[] Buffer = new byte[0x100];

    public static uint ReadUInt32BE(this Stream stream)
    {
        stream.Read(Buffer, 0, 4);
        var value = Unsafe.ReadUnaligned<uint>(ref Buffer[0]);

        if (BitConverter.IsLittleEndian)
            return SwapEndian(value);
        else
            return value;
    }

    public static ulong ReadUInt64BE(this Stream stream)
    {
        stream.Read(Buffer, 0, 8);
        var value = Unsafe.ReadUnaligned<ulong>(ref Buffer[0]);

        if (BitConverter.IsLittleEndian)
            return SwapEndian(value);
        else
            return value;
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
        if (BitConverter.IsLittleEndian)
            value = SwapEndian(value);

        Unsafe.WriteUnaligned(ref Buffer[0], value);
        stream.Write(Buffer, 0, 4);
    }

    public static void WriteUInt64BE(this Stream stream, ulong value)
    {
        if (BitConverter.IsLittleEndian)
            value = SwapEndian(value);

        Unsafe.WriteUnaligned(ref Buffer[0], value);
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

    private static uint SwapEndian(uint x)
    {
        x = (x >> 16) | (x << 16);
        return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
    }

    private static ulong SwapEndian(ulong x)
    {
        x = (x >> 32) | (x << 32);
        x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
        return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
    }
}
