namespace AresChroniclesDumper.FileSystem;

internal class SubStream : Stream
{
    public override bool CanRead => BaseStream.CanRead;
    public override bool CanSeek => BaseStream.CanSeek;
    public override bool CanWrite => false;
    public override long Length => End - Start;
    public override long Position
    {
        get => BaseStream.Position - Start;
        set
        {
            var offset = Start + value;
            if (offset < Start || offset > End)
                throw new IOException(nameof(Position));

            BaseStream.Position = offset;
        }
    }

    private Stream BaseStream;
    private readonly long Start;
    private readonly long End;

    public SubStream(Stream stream, long offset, long length)
    {
        BaseStream = stream;
        Start = offset;
        End = Math.Min(stream.Length, Start + length);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        count = Math.Min(count, (int)(Length - Position));

        if (count < 0)
            return 0;

        return BaseStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;
            case SeekOrigin.Current:
                Position += offset;
                break;
            case SeekOrigin.End:
                Position = Length - offset;
                break;
        }

        return Position;
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        BaseStream = null;
    }

    public override void Flush()
    {
        // unimplemented
    }

    public override void Close()
    {
        BaseStream = null;
    }
}
