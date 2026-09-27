using System.IO;

public class IOWriterFile : IOWriter
{
    private readonly FileStream   stream;
    private readonly BinaryWriter writer;

    public IOWriterFile(string path)
    {
        stream = new FileStream  (path, FileMode.Create, FileAccess.Write);
        writer = new BinaryWriter(stream);
    }

    public void WriteBool  (bool   value) { writer.Write(value); }
    public void WriteByte  (byte   value) { writer.Write(value); }
    public void WriteInt   (int    value) { writer.Write(value); }
    public void WriteLong  (long   value) { writer.Write(value); }
    public void WriteFloat (float  value) { writer.Write(value); }
    public void WriteString(string value) { writer.Write(value ?? ""); }

    public void Dispose()
    {
        writer.Dispose();
        stream.Dispose();
    }
}