using System.IO;

public class IOReaderFile : IOReader
{
    private readonly FileStream   stream;
    private readonly BinaryReader reader;

    public IOReaderFile(string path)
    {
        stream = new FileStream  (path, FileMode.Open, FileAccess.Read);
        reader = new BinaryReader(stream);
    }

    public bool   ReadBool  () { return reader.ReadBoolean(); }
    public byte   ReadByte  () { return reader.ReadByte   (); }
    public int    ReadInt   () { return reader.ReadInt32  (); }
    public long   ReadLong  () { return reader.ReadInt64  (); }
    public float  ReadFloat () { return reader.ReadSingle (); }
    public string ReadString() { return reader.ReadString (); }

    public void Dispose()
    {
        reader.Dispose();
        stream.Dispose();
    }
}
