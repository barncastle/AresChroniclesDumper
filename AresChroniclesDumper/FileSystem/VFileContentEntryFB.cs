using FlatBuffers;

namespace AresChroniclesDumper.FileSystem;

public struct VFileContentEntryFB : IFlatbufferObject
{
    private Table __p;

    public ByteBuffer ByteBuffer => __p.bb;

    public string ContentFile
    {
        get
        {
            var num = __p.__offset(4);
            if (num == 0)
                return null;

            return __p.__string(num + __p.bb_pos);
        }
    }

    public int EntrysLength
    {
        get
        {
            var num = __p.__offset(6);
            if (num == 0)
                return 0;

            return __p.__vector_len(num);
        }
    }

    public static void ValidateVersion()
    {
        FlatBufferConstants.FLATBUFFERS_1_12_0();
    }

    public static VFileContentEntryFB GetRootAsVFileContentEntryFB(ByteBuffer _bb)
    {
        return GetRootAsVFileContentEntryFB(_bb, default);
    }

    public static VFileContentEntryFB GetRootAsVFileContentEntryFB(ByteBuffer _bb, VFileContentEntryFB obj)
    {
        return obj.Assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
    }

    public void __init(int _i, ByteBuffer _bb)
    {
        __p = new Table(_i, _bb);
    }

    public VFileContentEntryFB Assign(int _i, ByteBuffer _bb)
    {
        __init(_i, _bb);
        return this;
    }

    public ArraySegment<byte>? GetContentFileBytes()
    {
        return __p.__vector_as_arraysegment(4);
    }

    public byte[] GetContentFileArray()
    {
        return __p.__vector_as_array<byte>(4);
    }

    public VFileEntryFB? Entrys(int j)
    {
        var num = __p.__offset(6);
        if (num == 0)
            return null;

        return default(VFileEntryFB).Assign(__p.__indirect(__p.__vector(num) + (j * 4)), __p.bb);
    }

    public static Offset<VFileContentEntryFB> CreateVFileContentEntryFB(FlatBufferBuilder builder, StringOffset contentFileOffset = default, VectorOffset entrysOffset = default)
    {
        builder.StartTable(2);
        AddEntrys(builder, entrysOffset);
        AddContentFile(builder, contentFileOffset);
        return EndVFileContentEntryFB(builder);
    }

    public static void StartVFileContentEntryFB(FlatBufferBuilder builder)
    {
        builder.StartTable(2);
    }

    public static void AddContentFile(FlatBufferBuilder builder, StringOffset contentFileOffset)
    {
        builder.AddOffset(0, contentFileOffset.Value, 0);
    }

    public static void AddEntrys(FlatBufferBuilder builder, VectorOffset entrysOffset)
    {
        builder.AddOffset(1, entrysOffset.Value, 0);
    }

    public static VectorOffset CreateEntrysVector(FlatBufferBuilder builder, Offset<VFileEntryFB>[] data)
    {
        builder.StartVector(4, data.Length, 4);
        for (var num = data.Length - 1; num >= 0; num--)
        {
            builder.AddOffset(data[num].Value);
        }
        return builder.EndVector();
    }

    public static VectorOffset CreateEntrysVectorBlock(FlatBufferBuilder builder, Offset<VFileEntryFB>[] data)
    {
        builder.StartVector(4, data.Length, 4);
        builder.Add(data);
        return builder.EndVector();
    }

    public static void StartEntrysVector(FlatBufferBuilder builder, int numElems)
    {
        builder.StartVector(4, numElems, 4);
    }

    public static Offset<VFileContentEntryFB> EndVFileContentEntryFB(FlatBufferBuilder builder)
    {
        return new Offset<VFileContentEntryFB>(builder.EndTable());
    }
}
