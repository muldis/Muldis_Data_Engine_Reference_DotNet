namespace Muldis.Object_Notation_Processor_Reference_Util.Processor;

public class Duplicate : IProcessor
{
    public Duplicate()
    {
    }

    public void process(Stream stream_in, Stream stream_out)
    {
        // Note that Stream.ReadByte()
        // returns one of 0..255 when there is another octet
        // and it returns -1 when there is none / the end of stream was passed.
        Int32 octet_as_int = stream_in.ReadByte();
        while (octet_as_int >= 0)
        {
            Byte octet = (Byte)octet_as_int;
            stream_out.WriteByte(octet);
            octet_as_int = stream_in.ReadByte();
        }
    }
}
