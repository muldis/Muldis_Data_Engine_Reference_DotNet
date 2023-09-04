namespace Muldis.Object_Notation_Processor_Reference_Util.Processor;

using Muldis.Object_Notation_Processor_Reference;

public class Duplicate : IProcessor
{
    public Duplicate()
    {
    }

    public void process(Stream stream_in, Stream stream_out)
    {
        Repeatable_Octet_Input_Stream @in
            = new Repeatable_Octet_Input_Stream(stream_in);
        Int32 octet_as_int = @in.read_octet();
        while (octet_as_int >= 0)
        {
            Byte octet = (Byte)octet_as_int;
            stream_out.WriteByte(octet);
            octet_as_int = @in.read_octet();
        }
    }
}
