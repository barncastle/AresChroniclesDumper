using FlatBuffers;

namespace AresChroniclesDumper.FileSystem;

public struct VFileEntryFB : IFlatbufferObject
{
    private Table __p;

    public ByteBuffer ByteBuffer => __p.bb;

    public string Path
    {
        get
        {
            var num = __p.__offset(4);
            if (num == 0)
                return null;

            return __p.__string(num + __p.bb_pos);
        }
    }

    public ulong Offset
    {
        get
        {
            var num = __p.__offset(6);
            if (num == 0)
                return 0uL;

            return __p.bb.GetUlong(num + __p.bb_pos);
        }
    }

    public uint Length
    {
        get
        {
            var num = __p.__offset(8);
            if (num == 0)
                return 0u;

            return __p.bb.GetUint(num + __p.bb_pos);
        }
    }

    public static void ValidateVersion()
    {
        FlatBufferConstants.FLATBUFFERS_1_12_0();
    }

    public static VFileEntryFB GetRootAsVFileEntryFB(ByteBuffer _bb)
    {
        return GetRootAsVFileEntryFB(_bb, default);
    }

    public static VFileEntryFB GetRootAsVFileEntryFB(ByteBuffer _bb, VFileEntryFB obj)
    {
        return obj.Assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
    }

    public void __init(int _i, ByteBuffer _bb)
    {
        __p = new Table(_i, _bb);
    }

    public VFileEntryFB Assign(int _i, ByteBuffer _bb)
    {
        __init(_i, _bb);
        return this;
    }

    public ArraySegment<byte>? GetPathBytes()
    {
        return __p.__vector_as_arraysegment(4);
    }

    public byte[] GetPathArray()
    {
        return __p.__vector_as_array<byte>(4);
    }

    public static Offset<VFileEntryFB> CreateVFileEntryFB(FlatBufferBuilder builder, StringOffset pathOffset = default, ulong offset = 0uL, uint length = 0u)
    {
        builder.StartTable(3);
        AddOffset(builder, offset);
        AddLength(builder, length);
        AddPath(builder, pathOffset);
        return EndVFileEntryFB(builder);
    }

    public static void StartVFileEntryFB(FlatBufferBuilder builder)
    {
        builder.StartTable(3);
    }

    public static void AddPath(FlatBufferBuilder builder, StringOffset pathOffset)
    {
        builder.AddOffset(0, pathOffset.Value, 0);
    }

    public static void AddOffset(FlatBufferBuilder builder, ulong offset)
    {
        builder.AddUlong(1, offset, 0uL);
    }

    public static void AddLength(FlatBufferBuilder builder, uint length)
    {
        builder.AddUint(2, length, 0u);
    }

    public static Offset<VFileEntryFB> EndVFileEntryFB(FlatBufferBuilder builder)
    {
        return new Offset<VFileEntryFB>(builder.EndTable());
    }
}
