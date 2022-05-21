using FlatBuffers;

namespace AresChroniclesDumper.FileSystem;

public struct VFileContentFB : IFlatbufferObject
{
    private Table __p;

    public ByteBuffer ByteBuffer => __p.bb;

    public int EntrysLength
    {
        get
        {
            var num = __p.__offset(4);
            if (num == 0)
                return 0;

            return __p.__vector_len(num);
        }
    }

    public int FileEntryCount
    {
        get
        {
            var num = __p.__offset(6);
            if (num == 0)
                return 0;

            return __p.bb.GetInt(num + __p.bb_pos);
        }
    }

    public static void ValidateVersion()
    {
        FlatBufferConstants.FLATBUFFERS_1_12_0();
    }

    public static VFileContentFB GetRootAsVFileContentFB(ByteBuffer _bb)
    {
        return GetRootAsVFileContentFB(_bb, default);
    }

    public static VFileContentFB GetRootAsVFileContentFB(ByteBuffer _bb, VFileContentFB obj)
    {
        return obj.Assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
    }

    public void __init(int _i, ByteBuffer _bb)
    {
        __p = new Table(_i, _bb);
    }

    public VFileContentFB Assign(int _i, ByteBuffer _bb)
    {
        __init(_i, _bb);
        return this;
    }

    public VFileContentEntryFB? Entrys(int j)
    {
        var num = __p.__offset(4);
        if (num == 0)
            return null;

        return default(VFileContentEntryFB).Assign(__p.__indirect(__p.__vector(num) + (j * 4)), __p.bb);
    }

    public static Offset<VFileContentFB> CreateVFileContentFB(FlatBufferBuilder builder, VectorOffset entrysOffset = default, int fileEntryCount = 0)
    {
        builder.StartTable(2);
        AddFileEntryCount(builder, fileEntryCount);
        AddEntrys(builder, entrysOffset);
        return EndVFileContentFB(builder);
    }

    public static void StartVFileContentFB(FlatBufferBuilder builder)
    {
        builder.StartTable(2);
    }

    public static void AddEntrys(FlatBufferBuilder builder, VectorOffset entrysOffset)
    {
        builder.AddOffset(0, entrysOffset.Value, 0);
    }

    public static VectorOffset CreateEntrysVector(FlatBufferBuilder builder, Offset<VFileContentEntryFB>[] data)
    {
        builder.StartVector(4, data.Length, 4);
        for (var num = data.Length - 1; num >= 0; num--)
        {
            builder.AddOffset(data[num].Value);
        }
        return builder.EndVector();
    }

    public static VectorOffset CreateEntrysVectorBlock(FlatBufferBuilder builder, Offset<VFileContentEntryFB>[] data)
    {
        builder.StartVector(4, data.Length, 4);
        builder.Add(data);
        return builder.EndVector();
    }

    public static void StartEntrysVector(FlatBufferBuilder builder, int numElems)
    {
        builder.StartVector(4, numElems, 4);
    }

    public static void AddFileEntryCount(FlatBufferBuilder builder, int fileEntryCount)
    {
        builder.AddInt(1, fileEntryCount, 0);
    }

    public static Offset<VFileContentFB> EndVFileContentFB(FlatBufferBuilder builder)
    {
        return new Offset<VFileContentFB>(builder.EndTable());
    }

    public static void FinishVFileContentFBBuffer(FlatBufferBuilder builder, Offset<VFileContentFB> offset)
    {
        builder.Finish(offset.Value);
    }

    public static void FinishSizePrefixedVFileContentFBBuffer(FlatBufferBuilder builder, Offset<VFileContentFB> offset)
    {
        builder.FinishSizePrefixed(offset.Value);
    }
}
