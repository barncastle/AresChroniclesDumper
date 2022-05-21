using AresChroniclesDumper.Models;

namespace AresChroniclesDumper;

internal static class BundleConverter
{
    public const string BundleSignature = "UnityFS";

    private static readonly byte[] XOR_Key = new byte[] { 0x1E, 0x1E, 0x01, 0x01, 0xFC };

    public static bool IsAresBundle(Stream stream)
    {
        if (stream.Length < AresBundleHeader.HeaderSize)
            return false;

        stream.Position = 0;

        var header = new AresBundleHeader(stream);
        if (header.Signature != BundleSignature)
            return false;

        return ConvertAresHeader(header).Size == (ulong)stream.Length;
    }

    public static bool IsUnityBundle(Stream stream)
    {
        if (stream.Length < 34)
            return false;

        stream.Position = 0;

        var header = new UnityBundleHeader(stream);

        return header.Signature == BundleSignature &&
            header.Size == (ulong)stream.Length;
    }

    public static void ConvertAresToUnity(Stream stream)
    {
        using var ms = new MemoryStream((int)stream.Length + 0x20);

        stream.Position = 0;

        var ares_header = new AresBundleHeader(stream);
        var unity_header = ConvertAresHeader(ares_header);

        if ((unity_header.Flags & 0x80) != 0)
            throw new Exception("Unsupported kArchiveBlocksInfoAtTheEnd flag");

        if (unity_header.Size != (ulong)stream.Length)
            throw new Exception("BundleHeader.Size != Stream.Length");

        // adjust the size based on the new header
        unity_header.Size += unity_header.HeaderSize - AresBundleHeader.HeaderSize;

        // write our bundle header
        unity_header.Write(ms);

        // write the decrypted CompressedBlocksInfo
        var compressedBlocksInfo = ApplyCompressedBlocksInfoCipher(unity_header.CompressedBlocksInfoSize, stream);
        ms.Write(compressedBlocksInfo, 0, compressedBlocksInfo.Length);

        // copy the remainder of the bundle
        stream.CopyTo(ms);

        // overwrite the original stream
        stream.Position = 0;
        stream.SetLength(ms.Length);
        ms.WriteTo(stream);
    }

    public static void ConvertUnityToAres(Stream stream)
    {
        using var ms = new MemoryStream((int)stream.Length + 0x20);

        stream.Position = 0;

        var unity_header = new UnityBundleHeader(stream);
        var ares_header = ConvertUnityHeader(unity_header);

        if ((unity_header.Flags & 0x80) != 0)
            throw new Exception("Unsupported kArchiveBlocksInfoAtTheEnd flag");

        // write our bundle header
        ares_header.Write(ms);

        // write the decrypted CompressedBlocksInfo
        var compressedBlocksInfo = ApplyCompressedBlocksInfoCipher(unity_header.CompressedBlocksInfoSize, stream);
        ms.Write(compressedBlocksInfo, 0, compressedBlocksInfo.Length);

        // copy the remainder of the bundle
        stream.CopyTo(ms);

        // overwrite the original stream
        stream.Position = 0;
        stream.SetLength(ms.Length);
        ms.WriteTo(stream);
    }

    /// <summary>
    /// Converts a AresBundleHeader to a UnityBundleHeader
    /// </summary>
    /// <param name="stream"></param>
    public static UnityBundleHeader ConvertAresHeader(AresBundleHeader header)
    {
        var field5 = (((ulong)header.Field5 << 32) & 0xFFFFFF0000000000UL) | (header.Field5 & 0xFF);
        var size = ((ulong)header.Field4 << 32) ^ (field5 | ((ulong)header.Field2 << 8)) ^ header.Field1;

        return new UnityBundleHeader
        {
            Size = size,
            CompressedBlocksInfoSize = Decode(header.Field1) ^ header.Field4,
            UncompressedBlocksInfoSize = Decode(header.Field6) ^ header.Field1,
            Flags = Decode(header.Field4) ^ 0x70020017
        };
    }

    /// <summary>
    /// Converts a UnityBundleHeader to a AresBundleHeader
    /// </summary>
    /// <param name="header"></param>
    /// <returns></returns>
    public static AresBundleHeader ConvertUnityHeader(UnityBundleHeader header)
    {
        // calculate the new file size
        var size = header.Size - header.HeaderSize + AresBundleHeader.HeaderSize;

        var flag_block_mix = (((header.Flags & 0xFFFF) ^ 0x17) & 0xE000) << 5;
        flag_block_mix |= EncodeLo(header.Flags ^ 0x70020017u, header.Flags & 0xFFFF);
        flag_block_mix &= 0xFFFF;
        flag_block_mix ^= header.CompressedBlocksInfoSize & 0xFFFF;

        var field4 = Encode(header.Flags ^ 0x70020017, (header.Flags & 0xFFFF) ^ 0x17);
        var field1 = Encode(field4 ^ header.CompressedBlocksInfoSize, flag_block_mix);
        var size_1_4_mix = size ^ field1 ^ ((ulong)field4 << 32);

        var field5_2 = size_1_4_mix & 0xFFFFFF0000000000UL;
        field5_2 |= (size_1_4_mix >> 8) & 0xFFFFFFFF;
        field5_2 |= ((EncodeLo(field4 ^ header.CompressedBlocksInfoSize, flag_block_mix) ^ (size & 0xFF)) & 0xFF) << 32;

        return new AresBundleHeader
        {
            Field1 = field1,
            Field2 = (uint)(field5_2 & 0xFFFFFFFF),
            Field3 = 0,
            Field4 = field4,
            Field5 = (uint)(field5_2 >> 32),
            Field6 = Encode(field1 ^ header.UncompressedBlocksInfoSize, field1 ^ header.UncompressedBlocksInfoSize)
        };
    }

    /// <summary>
    /// Applies an XOR cipher to the Compressed Blocks Info using the <see cref="XOR_Key"/>
    /// </summary>
    /// <param name="header"></param>
    /// <param name="stream"></param>
    private static byte[] ApplyCompressedBlocksInfoCipher(uint compressedBlocksInfoSize, Stream stream)
    {
        var compressedBlocksInfo = new byte[compressedBlocksInfoSize];
        stream.Read(compressedBlocksInfo, 0, (int)compressedBlocksInfoSize);

        for (var i = 0; i < compressedBlocksInfoSize; i++)
            compressedBlocksInfo[i] ^= XOR_Key[i % 5];

        return compressedBlocksInfo;
    }

    private static uint Decode(uint x)
    {
        return (((x >> 24) | (x & 0x1FFC0000)) >> 5) | (((x & 0x3FF) | ((x & 0xFFFFFC00) << 11)) << 3);
    }

    private static uint Encode(uint x, uint y)
    {
        return (((x << 24) | (x & 0xFFE000)) << 5) | EncodeLo(x, y);
    }

    private static uint EncodeLo(uint x, uint y)
    {
        return ((y & 0x1FF8) | ((x >> 11) & 0x1FE000)) >> 3;
    }
}
